using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private RawImage mouse;
    [SerializeField] private GameObject[] bodyNotShow;
    public Vector3 camPos;

    private Vector3 direction;
    private InputSystem_Actions inputActions;
    private Vector3 inputDirection;

    private bool isGrounded;
    private bool isOnObject;
    private bool shouldJump;
    private bool first;

    private int currentNumPlayer = 0;

    private float looseSpeed = .7f;
    private float speed = 5f;
    private float maxSpeed = 8f;
    private float waitBegin = .05f;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            mouse.gameObject.SetActive(true);
        }
        StartCoroutine(RegisterPlayerNextFrame());
    }

    private void OnEnable()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Move.performed += GetDirection;
        inputActions.Player.Move.canceled += ctx => inputDirection = Vector3.zero;
        inputActions.Player.Jump.performed += ctx => shouldJump = true;
        inputActions.Player.Jump.canceled += ctx => shouldJump = false;
        inputActions.Player.Enable();

        NetworkManager.Singleton.OnClientConnectedCallback += Init;
        ActionManager.ActivateMovement += ActivateInput;
        ActionManager.DeactivateMovement += DeactivateInput;

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

        NetworkManager.Singleton.OnClientConnectedCallback -= Init;

        ActionManager.ActivateMovement -= ActivateInput;
        ActionManager.DeactivateMovement -= DeactivateInput;
    }

    private void DeactivateInput()
    {
        inputActions.Disable();
    }
    private void ActivateInput()
    {
        inputActions.Enable();
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;


        direction = transform.forward * inputDirection.z + transform.right * inputDirection.x;

        Jump();

        if (isGrounded)
        {
            direction = direction * moveSpeed;
            rb.linearVelocity = new Vector3(direction.x, rb.linearVelocity.y, direction.z);
        }
        else
        {
            direction = direction * moveSpeed * looseSpeed;
            rb.linearVelocity = new Vector3(
                Mathf.Lerp(rb.linearVelocity.x, direction.x, Time.fixedDeltaTime * speed),
                rb.linearVelocity.y,
                Mathf.Lerp(rb.linearVelocity.z, direction.z, Time.fixedDeltaTime * speed)
            );

        }

        Vector3 lOrigin = Camera.main.transform.position;
        Vector3 lDirection = -gameObject.transform.up;
        int mask = ~(1 << 7);
        RaycastHit hit;
        if (Physics.Raycast(lOrigin, lDirection, out hit, 2f, mask))
        {
            if (!isGrounded)
                isGrounded = true;
        }
        else if(isGrounded)
        {
            isGrounded = false;
        }

        Vector3 horizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        horizontal = Vector3.ClampMagnitude(horizontal, maxSpeed);
        rb.linearVelocity = new Vector3(horizontal.x, rb.linearVelocity.y, horizontal.z);
    }

    private IEnumerator RegisterPlayerNextFrame()
    {
        yield return null;
        ActionManager.addPlayer?.Invoke(this);
        if (IsServer)
        {
            yield return new WaitForSeconds(waitBegin);
            ActionManager.activatePlayer.Invoke();
        }
    }

    private void Init(ulong id)
    {
        if (!IsServer)
            return;
            
        currentNumPlayer += 1;
        if (currentNumPlayer == LobbyManager.instance.numPlayer)
        {
            ActionManager.activatePlayer.Invoke();
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
