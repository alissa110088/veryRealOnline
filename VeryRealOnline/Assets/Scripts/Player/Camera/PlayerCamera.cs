using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] AudioListener audioListener;
    void Start()
    {
        if (IsOwner)
            camera.enabled = true;
        else
            audioListener.enabled = false;
    }
}
