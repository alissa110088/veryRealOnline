using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Rigidbody rb;

    private Vector3 direction;
    private InputSystem_Actions inputActions;
    private Vector3 inputDirection;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        inputActions = new InputSystem_Actions();
        inputActions.Player.Move.performed += GetDirection;
        inputActions.Player.Move.canceled += ctx =>  inputDirection = Vector3.zero;
        inputActions.Player.Enable();
        Cursor.visible = false;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        inputActions.Dispose();
        inputActions.Disable();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        direction = transform.forward * inputDirection.z + transform.right * inputDirection.x;
        rb.AddForce(direction * moveSpeed);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 10f);
        Debug.Log(rb.linearVelocity);

    }

    private void GetDirection(InputAction.CallbackContext ctx)
    {
        if(!IsOwner)
            return;

        inputDirection = ctx.ReadValue<Vector2>();
        inputDirection = new Vector3(inputDirection.x, 0f, inputDirection.y);
        inputDirection = inputDirection.normalized;
    }
}
