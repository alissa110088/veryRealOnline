using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Canvas currentCanvas;
    [SerializeField] private NetworkObject lobbyPrefab;

    private void Awake()
    {
        hostButton.onClick.AddListener(OnClickHost);
        serverButton.onClick.AddListener(() => NetworkManager.Singleton.StartServer());
        clientButton.onClick.AddListener(OnClickClient);
    }

    private void OnClickHost()
    {
        NetworkManager.Singleton.StartHost();

        NetworkObject lobby = Instantiate(lobbyPrefab);
        lobby.Spawn();

        currentCanvas.gameObject.SetActive(false);
    }

    private void OnClickClient()
    {
        NetworkManager.Singleton.StartClient();
        currentCanvas.gameObject.SetActive(false);
    }
}
