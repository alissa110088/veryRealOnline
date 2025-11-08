using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCanvas : MonoBehaviour
{
    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void OnPressed()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
