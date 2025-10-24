using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private List<PlayerNetwork> players = new List<PlayerNetwork>();
    private float ChanceToBeSeeker = 0.2f;

    public override void OnNetworkSpawn()
    {
        ActionManager.addPlayer += AddPlayer;
        ActionManager.activatePlayer += ActivateAllPlayer;
    }

    private void AddPlayer(PlayerNetwork pNetwork)
    {
        players.Add(pNetwork);
        pNetwork.enabled = false;
    }

    private void ActivateAllPlayer()
    {
        foreach (PlayerNetwork network in players)
        {
            network.enabled = true;
        }
        if (IsServer)
            GetShuffleListServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void GetShuffleListServerRpc()
    {
        int howMany = Mathf.RoundToInt(players.Count * ChanceToBeSeeker);

        if (howMany == 0 && players.Count > 1)
        {
            howMany = 1;
        }

        PlayerNetwork[] lListShuffled = new PlayerNetwork[players.Count];
        foreach (PlayerNetwork network in players)
        {
            while (!lListShuffled.Contains(network))
            {
                int lIndex = Random.Range(0, players.Count);
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
            PlayerNetwork player = players.Find(p => p.OwnerClientId == clientId);
            if (player == null) continue;

            if (i < howMany)
                ActionManager.GivePlayerRole?.Invoke(EnumPlayerState.seeker, player.gameObject);
            else
                ActionManager.GivePlayerRole?.Invoke(EnumPlayerState.hider, player.gameObject);

            i++;
        }
    }
}
