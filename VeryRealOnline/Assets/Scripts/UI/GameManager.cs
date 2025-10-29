using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public List<PlayerNetwork> playersAlive = new List<PlayerNetwork>();
    public List<PlayerNetwork> playersDead = new List<PlayerNetwork>();    

    private float ChanceToBeSeeker = 0.2f;

    public static GameManager instance;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            instance = this;
        }
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
    }
}
