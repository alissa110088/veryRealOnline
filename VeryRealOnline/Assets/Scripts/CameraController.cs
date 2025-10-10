using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Camera camera;

    private const string xAxis = "Mouse X";
    private const string yAxis = "Mouse Y";

    private Vector2 rotation = Vector2.zero;

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;
        rotation.x += Input.GetAxis(xAxis) * 10f;
        rotation.y += (Input.GetAxis(yAxis) * 5f);
        rotation.y = Mathf.Clamp(rotation.y, -26f, -20f);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotation.x, Vector2.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(rotation.y, Vector2.left);

        transform.localRotation = xQuaternion ;
        camera.transform.localRotation = yQuaternion ;
    }
}
