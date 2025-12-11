using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private List<PlayerNetwork> playersAlive = new List<PlayerNetwork>();
    private List<PlayerNetwork> playersDead = new List<PlayerNetwork>();

    private float chanceToBeSeeker = 0.2f;
    private float second = 60f;
    private bool startTimer;
    private bool chatAlreadySpawned;
    private string hiderTag = "hider";
    private int numPlayerLeft = 0;
    private int currentPlayerConnected = 0;

    [SerializeField] private NetworkVariable<float> time = new NetworkVariable<float>(600f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI playerLeftText;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject seekerWin;
    [SerializeField] private GameObject hiderWin;
    [SerializeField] private GameObject chatBox;
    [SerializeField] private GameObject Ui;
    [SerializeField] private GameObject lobby;
    [SerializeField] private GameObject anchorSeeker;
    [SerializeField] private GameObject anchorHider;


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


        if (IsServer)
            TimerServer();
    }
    public override void OnNetworkSpawn()
    {
        ActionManager.addPlayer += AddPlayer;
        NetworkManager.Singleton.OnClientConnectedCallback += ExecInit;
    }

    public void OnDestroy()
    {
        ActionManager.addPlayer -= AddPlayer;
        NetworkManager.Singleton.OnClientConnectedCallback -= ExecInit;
    }

    private void ExecInit(ulong pPlayer)
    {
        if (IsServer)
            Init();
    }

    private void Init()
    {
        if (!IsServer)
            return;
        currentPlayerConnected += 1;

        if (currentPlayerConnected == LobbyManager.instance.numPlayer)
            StartCoroutine(waitBeforeStart());
    }

    private IEnumerator waitBeforeStart()
    {
        yield return new WaitForSeconds(1);

        ActivateAllPlayer();
    }
    private void AddPlayer(PlayerNetwork pNetwork)
    {
        playersAlive.Add(pNetwork);
    }

    public void RemovePlayer(PlayerNetwork pNetwork)
    {
        playersAlive.Remove(pNetwork);
        playersDead.Add(pNetwork);
        UpdatePlayerRpc(true);
        foreach (PlayerNetwork lNet in playersAlive)
        {
            if (lNet.gameObject.CompareTag(hiderTag))
                return;
        }

        seekerWinRpc();
    }
    [Rpc(SendTo.Everyone)]
    private void hideUiRPC()
    {
        lobby.SetActive(false);
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
        hideUiRPC();
        for (int i = 0; i < playersAlive.Count; i++)
        {
            playersAlive[i].gameObject.transform.position = new Vector3(i, 0f, 0f);
            playersAlive[i].gameObject.SetActive(true);
        }
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
        if (!chatAlreadySpawned)
        {
            GameObject chat = Instantiate(chatBox, Ui.transform);
            NetworkObject networkObject = chat.GetComponent<NetworkObject>();
            networkObject.Spawn();
            chatAlreadySpawned = true;
        }

        int howMany = Mathf.RoundToInt(playersAlive.Count * chanceToBeSeeker);

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
        numPlayerLeft = 0;
        foreach (ulong clientId in playerIds)
        {
            PlayerNetwork player = playersAlive.Find(p => p.OwnerClientId == clientId);
            if (player == null)
            {
                continue;
            }

            if (i < howMany)
                ActionManager.GivePlayerRole?.Invoke(EnumPlayerState.seeker, player.gameObject, anchorSeeker.transform.position);
            else
            {
                ActionManager.GivePlayerRole?.Invoke(EnumPlayerState.hider, player.gameObject, anchorHider.transform.position);
                numPlayerLeft++;
            }

            i++;
        }

        UpdatePlayerRpc();
        startTimer = true;
    }

    [Rpc(SendTo.Everyone)]
    private void UpdatePlayerRpc(bool pRemovePlayer = false)
    {
        if (pRemovePlayer)
        {
            numPlayerLeft--;
        }
        playerLeftText.text = "hider left: " + numPlayerLeft;
    }

    private void TimerServer()
    {
        time.Value -= Time.deltaTime;
        int lMinute = Mathf.FloorToInt(time.Value / second);
        int lSeconde = Mathf.FloorToInt(time.Value % second);

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
