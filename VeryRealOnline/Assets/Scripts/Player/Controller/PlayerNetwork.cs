using DG.Tweening;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float sprintSpeed = 1.0f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private RawImage mouse;
    [SerializeField] private GameObject[] bodyNotShow;
    [SerializeField] private Slider sprintBar;
    [SerializeField] private Camera cam;
    [SerializeField] private float sprintLoose = 0.2f;
    [SerializeField] private GameObject sliderCanvas;
    public Vector3 camPos;

    private Vector3 direction;
    private InputSystem_Actions inputActions;
    private Vector3 inputDirection;

    private bool isGrounded;
    private bool shouldJump;
    private bool first;
    private bool isSprinting;
    private bool lockSprint;

    private int currentNumPlayer = 0;

    private float sprintSpeedMultipliyer = 1.5f;
    private float maxSpeed = 30f;
    private float waitBegin = .05f;
    private float baseFov = 60f;
    private float sprintFov = 80f;
    private float tweenFov = .3f;
    private float upSprint = .1f;
    private float minToSprint = .3f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            mouse.gameObject.SetActive(true);
            sliderCanvas.SetActive(true);
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

    private void Update()
    {
        if (!IsOwner) return;

        SprintCheck();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        direction = transform.forward * inputDirection.z + transform.right * inputDirection.x;

        Jump();
        ApplyMove();
        CheckGround();
    }

    private void ApplyMove()
    {
        direction = direction * moveSpeed * sprintSpeed;
        direction = Vector3.ClampMagnitude(direction, maxSpeed);
        rb.linearVelocity = new Vector3(direction.x, rb.linearVelocity.y, direction.z);
    }

    private void CheckGround()
    {
        Vector3 lOrigin = Camera.main.transform.position;
        Vector3 lDirection = -gameObject.transform.up;
        int mask = ~(1 << 7);
        RaycastHit hit;
        if (Physics.Raycast(lOrigin, lDirection, out hit, 2f, mask))
        {
            if (!isGrounded)
            {
                isGrounded = true;
                sprintSpeed = 1;
            }
        }
        else if (isGrounded)
        {
            isGrounded = false;
        }
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
    private void ChangeFov(float newFov = 100f, float transitionDuration = 0.5f)
    {
        DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, newFov, transitionDuration);
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

    private void SprintCheck()
    {
        if (sprintBar.value <= 0.01f)
            lockSprint = true;

        if (lockSprint && sprintBar.value >= minToSprint)
            lockSprint = false;

        if (inputActions.Player.Sprint.IsPressed() && sprintBar.value - sprintLoose * Time.deltaTime >= 0f && !lockSprint)
        {
            sprintBar.value -= sprintLoose * Time.deltaTime;

            if (sprintSpeed != sprintSpeedMultipliyer)
            {
                ChangeFov(sprintFov, tweenFov);
                sprintSpeed = sprintSpeedMultipliyer;
                isSprinting = true;
            }
        }
        else
        {
            if (isSprinting)
            {
                ChangeFov(baseFov, tweenFov);
                isSprinting = false;
                sprintSpeed = 1f;
            }
            if (sprintBar.value != 1f)
            {
                sprintBar.value += upSprint * Time.deltaTime;
            }
        }
    }

    private void Jump()
    {
        if (!IsOwner) return;

        Debug.Log(shouldJump);
        if (isGrounded && shouldJump)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
