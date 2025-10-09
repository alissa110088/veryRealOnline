using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] AudioListener audioListener;
    private void Awake()
    {

    }
    void Start()
    {
        if (IsOwner)
            camera.enabled = true;
        else
            audioListener.enabled = false;
    }
}
