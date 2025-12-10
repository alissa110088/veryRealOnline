using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private float sensibilityX = 2f;
    [SerializeField] private float sensibilityY = 1f;
    [SerializeField] private float maxUpCamera = 90f;
    [SerializeField] private float minDownCamera = -90f;

    private float xAxis = 0;
    private float yAxis = 0;

    private Vector2 rotation = Vector2.zero;
    private InputSystem_Actions inputActions;
    private void OnEnable()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Look.performed += GetTheAxis;
        inputActions.Player.Look.canceled += PutAxisZero;
        inputActions.Enable();

    }

    private void OnDestroy()
    {
        inputActions.Player.Look.performed -= GetTheAxis;
    }
    private void LateUpdate()
    {
        if (!IsOwner) return;
        rotation.x += xAxis * sensibilityX;
        rotation.y += yAxis * sensibilityY;
        rotation.y = Mathf.Clamp(rotation.y, minDownCamera, maxUpCamera);

        transform.localRotation = Quaternion.Euler(0f, rotation.x, 0f);

        camera.transform.localRotation = Quaternion.Euler(-rotation.y, 0f, 0f);
    }

    private void PutAxisZero(InputAction.CallbackContext ctx)
    {
        xAxis = 0f;
        yAxis = 0f;
    }
    private void GetTheAxis(InputAction.CallbackContext ctx)
    {
        xAxis = ctx.ReadValue<Vector2>().x;
        yAxis = ctx.ReadValue<Vector2>().y;
    }
}
