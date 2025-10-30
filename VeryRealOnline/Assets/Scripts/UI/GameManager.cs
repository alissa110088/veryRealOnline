using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private List<PlayerNetwork> playersAlive = new List<PlayerNetwork>();
    private List<PlayerNetwork> playersDead = new List<PlayerNetwork>();

    private float ChanceToBeSeeker = 0.2f;
    private bool startTimer;

    [SerializeField] private NetworkVariable<float> time = new NetworkVariable<float>(600f ,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject seekerWin;
    [SerializeField] private GameObject hiderWin;

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (!startTimer)
            return;

        TimerServerRpc();
    }
    public override void OnNetworkSpawn()
    {
        ActionManager.addPlayer += AddPlayer;
        ActionManager.activatePlayer += ActivateAllPlayer;
    }

    private void AddPlayer(PlayerNetwork pNetwork)
    {
        playersAlive.Add(pNetwork);
        pNetwork.enabled = false;
    }

    public void RemovePlayer(PlayerNetwork pNetwork)
    {
        playersAlive.Remove(pNetwork);
        playersDead.Add(pNetwork);

        foreach (PlayerNetwork lNet in playersAlive)
        {
            if (lNet.gameObject.CompareTag("hider"))
                return;
        }

        seekerWinRpc();
    }

    [Rpc(SendTo.Everyone)]

    private void seekerWinRpc()
    {
        startTimer = false;
        text.enabled = false;
        winCanvas.SetActive(true);
        seekerWin.SetActive(true);
    }

    private void ActivateAllPlayer()
    {
        foreach (PlayerNetwork network in playersAlive)
        {
            network.enabled = true;
        }
        if (IsServer)
            GetShuffleListServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void GetShuffleListServerRpc()
    {
        int howMany = Mathf.RoundToInt(playersAlive.Count * ChanceToBeSeeker);

        if (howMany == 0 && playersAlive.Count > 1)
        {
            howMany = 1;
        }

        PlayerNetwork[] lListShuffled = new PlayerNetwork[playersAlive.Count];
        foreach (PlayerNetwork network in playersAlive)
        {
            while (!lListShuffled.Contains(network))
            {
                int lIndex = Random.Range(0, playersAlive.Count);
                if (lListShuffled[lIndex] == null)
                {
                    lListShuffled[lIndex] = network;
                }
            }
        }

        ulong[] playerIds = new ulong[lListShuffled.Length];
        for (int i = 0; i < lListShuffled.Length; i++)
            playerIds[i] = lListShuffled[i].OwnerClientId;

        GivePlayerRoleClientRpc(playerIds, howMany);
    }

    [ClientRpc]
    private void GivePlayerRoleClientRpc(ulong[] playerIds, int howMany)
    {
        int i = 0;
        foreach (ulong clientId in playerIds)
        {
            PlayerNetwork player = playersAlive.Find(p => p.OwnerClientId == clientId);
            if (player == null) continue;

            if (i < howMany)
                ActionManager.GivePlayerRole?.Invoke(EnumPlayerState.seeker, player.gameObject);
            else
                ActionManager.GivePlayerRole?.Invoke(EnumPlayerState.hider, player.gameObject);

            i++;
        }

        startTimer = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void TimerServerRpc()
    {
        time.Value -= Time.deltaTime;
        int lMinute = Mathf.FloorToInt(time.Value / 60f);
        int lSeconde = Mathf.FloorToInt(time.Value % 60f);

        UpdateTextRpc(lMinute, lSeconde);
    }

    [Rpc(SendTo.Everyone)]

    private void UpdateTextRpc(int pMinute, int pSeconde)
    {
        text.text = pMinute.ToString() + " : " + pSeconde.ToString();

        if (pMinute == 0 && pSeconde == 0)
        {
            startTimer = false;
            winCanvas.SetActive(true);
            hiderWin.SetActive(true);
        }

    }
}
