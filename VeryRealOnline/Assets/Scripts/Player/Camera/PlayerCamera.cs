using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private AudioListener audioListener;
    void Start()
    {
        if (IsOwner)
            camera.enabled = true;
        else
            audioListener.enabled = false;
    }
}
