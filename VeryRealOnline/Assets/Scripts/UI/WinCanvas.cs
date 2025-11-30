using Unity.Netcode;
using Unity.Services.Authentication;
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
        gameObject.SetActive(false);
        NetworkManager.Singleton.Shutdown();

        if (AuthenticationService.Instance.IsSignedIn)
            AuthenticationService.Instance.SignOut();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
