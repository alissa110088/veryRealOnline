using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private float sensibilityX = 2f;
    [SerializeField] private float sensibilityY = 1f;
    [SerializeField] private float maxUpCamera = 90f;
    [SerializeField] private float minDownCamera = -90f;

    private const string xAxis = "Mouse X";
    private const string yAxis = "Mouse Y";

    private Vector2 rotation = Vector2.zero;

    private void LateUpdate()
    {
        if (!IsOwner) return;
        rotation.x += Input.GetAxis(xAxis) * sensibilityX;
        rotation.y += (Input.GetAxis(yAxis) * sensibilityY);
        rotation.y = Mathf.Clamp(rotation.y, minDownCamera, maxUpCamera);

        transform.localRotation = Quaternion.Euler(0f, rotation.x, 0f);

        camera.transform.localRotation = Quaternion.Euler(-rotation.y, 0f, 0f);
    }
}
