using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private float sensibilityX = 10f;
    [SerializeField] private float sensibilityY = 5f;
    [SerializeField] private float maxUpCamera = 90f;
    [SerializeField] private float minDownCamera = -90f;

    private const string xAxis = "Mouse X";
    private const string yAxis = "Mouse Y";

    private Vector2 rotation = Vector2.zero;

    private void LateUpdate()
    {
        if(!IsOwner) return;
        rotation.x += Input.GetAxis(xAxis) * sensibilityX;
        rotation.y += (Input.GetAxis(yAxis) * sensibilityY);
        rotation.y = Mathf.Clamp(rotation.y, minDownCamera, maxUpCamera);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotation.x, Vector2.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(rotation.y, Vector2.left);

        transform.localRotation = xQuaternion ;
        camera.transform.localRotation = yQuaternion ;
    }
}
