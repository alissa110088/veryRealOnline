using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyWait : NetworkBehaviour
{
    [SerializeField] private TMP_Text numPlayerText;
    [SerializeField] private Button startButton;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += ClientCount;
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientCount;
        }
        else
        {
            startButton.gameObject.SetActive(false);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (!NetworkManager.Singleton.IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= ClientCount;
        NetworkManager.Singleton.OnClientDisconnectCallback -= ClientCount;
    }

    private void ClientCount(ulong id)
    {
       ClientCountClientRPC(NetworkManager.Singleton.ConnectedClients.Count);
    }

    [ClientRpc]

    private void ClientCountClientRPC(int count)
    {
        numPlayerText.text = count.ToString() + " player connected";
    }


    public void OnPressed()
    {

        if (!IsServer)
            return;


        RemoveUiClientRPC();
    }

    [ClientRpc]
    private void RemoveUiClientRPC()
    {
        ActionManager.activatePlayer?.Invoke();


        if (IsServer)
        {
            NetworkObject.Despawn();
            Destroy(gameObject);
        }
        else
            gameObject.SetActive(false);

    }

}
