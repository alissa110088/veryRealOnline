using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float jumpForce = 5f;
    public Vector3 camPos;

    private Vector3 direction;
    private InputSystem_Actions inputActions;
    private Vector3 inputDirection;

    private bool isGrounded;
    private bool shouldJump;
    private bool first;

    private const string groundLayerName = "Ground";
    private const string ObstacleLayerName = "Obstacle";

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        StartCoroutine(RegisterPlayerNextFrame());

    }

    private void OnEnable()
    {
        transform.position = new Vector3(0f, 0f, 0f);


        if (!first)
            return;

        inputActions = new InputSystem_Actions();
        inputActions.Player.Move.performed += GetDirection;
        inputActions.Player.Move.canceled += ctx => inputDirection = Vector3.zero;
        inputActions.Player.Jump.performed += ctx => shouldJump = true;
        inputActions.Player.Jump.canceled += ctx => shouldJump = false;
        inputActions.Player.Enable();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        if (!first)
        {
            first = true;

            return;
        }

        inputActions.Dispose();
        inputActions.Disable();
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        direction = transform.forward * inputDirection.z + transform.right * inputDirection.x;

        //TODO ne marche pas a regler
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction.normalized, out hit, 1f))
        {
            Vector3 push = Vector3.Cross(hit.normal, Vector3.up);
            rb.position += push * 0.01f;
        }

        Jump();

        if (isGrounded)
        {
            direction = direction * moveSpeed;
            rb.linearVelocity = new Vector3(direction.x, rb.linearVelocity.y, direction.z);
        }
        else
        {
            direction = direction * moveSpeed * .7f;
            rb.linearVelocity = new Vector3(
                Mathf.Lerp(rb.linearVelocity.x, direction.x, Time.fixedDeltaTime * 5f),
                rb.linearVelocity.y,
                Mathf.Lerp(rb.linearVelocity.z, direction.z, Time.fixedDeltaTime * 5f)
            );

        }

        Vector3 horizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        horizontal = Vector3.ClampMagnitude(horizontal, 8f);
        rb.linearVelocity = new Vector3(horizontal.x, rb.linearVelocity.y, horizontal.z);
    }

    private IEnumerator RegisterPlayerNextFrame()
    {
        yield return null;
        ActionManager.addPlayer?.Invoke(this);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayerName) && !isGrounded)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!IsOwner) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayerName) && isGrounded)
        {
            isGrounded = false;
        }
    }

    private void GetDirection(InputAction.CallbackContext ctx)
    {
        if (!IsOwner)
            return;

        inputDirection = ctx.ReadValue<Vector2>();
        inputDirection = new Vector3(inputDirection.x, 0f, inputDirection.y);
        inputDirection = inputDirection.normalized;
    }

    private void Jump()
    {
        if (!IsOwner) return;

        if (isGrounded && shouldJump)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
