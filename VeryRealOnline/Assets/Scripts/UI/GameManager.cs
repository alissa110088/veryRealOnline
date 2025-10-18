using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private List<PlayerNetwork> Players = new List<PlayerNetwork>();

    public override void OnNetworkSpawn()
    {
        ActionManager.addPlayer += AddPlayer;
        ActionManager.activatePlayer += ActivateAllPlayer;
    }

    private void AddPlayer(PlayerNetwork pNetwork)
    {
        Players.Add(pNetwork);   
        pNetwork.enabled = false;
    }

    private void ActivateAllPlayer()
    {
        foreach (PlayerNetwork network in Players)
        {
            network.enabled = true;
        }
    }
}
