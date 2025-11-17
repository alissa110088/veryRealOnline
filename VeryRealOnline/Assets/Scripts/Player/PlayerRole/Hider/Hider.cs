using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Hider : NetworkBehaviour
{
    public Camera cam;

    //[SerializeField] private LayerMask objectLayer;
    [SerializeField] private Texture green;
    [SerializeField] private Texture darkGreen;
    [SerializeField] private Texture red;
    [SerializeField] private RawImage mouse;

    private float distanceToGrab = 5f;
    private float SmoothMovementFourniture = 15f;
    private LayerMask objectLayer;
    private InputSystem_Actions inputActions;
    private GameObject objectInHand = null;
    private float grabDistance;
    private bool canGrabItem;
    private RaycastHit hit;
    private Rigidbody rbObject;
    private NetworkObject objectNetwork;
    private GameObject focusedObject;
    private Vector3 lCurrentAxis = new Vector3(0, 0, 1);


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        objectLayer = LayerMask.GetMask("Obstacle");

        inputActions = new InputSystem_Actions();

        inputActions.Player.Interact.started += GetFurniture;
        inputActions.Player.Interact.canceled += LetGo;
        inputActions.Player.RotateChange.performed += Rotate;
        inputActions.Player.rotate.started += ChangeAxis;
    }

    private void OnEnable()
    {
        if (IsOwner)
        {
            inputActions.Player.Enable();
        }
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        inputActions.Dispose();
        inputActions.Disable();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (objectInHand != null)
        {
            MoveFurniture();
        }

        Vector3 lOrigin = cam.transform.position;
        Vector3 lDirection = cam.transform.forward;

        //Debug.DrawRay(lOrigin, lDirection * 5f, Color.red, 0.1f);


        if (objectInHand == null)
        {
            if (Physics.Raycast(lOrigin, lDirection, out hit, distanceToGrab, objectLayer))
            {
                focusedObject = hit.transform.gameObject;
                canGrabItem = true;
                mouse.texture = darkGreen;
            }
            else
            {
                if (canGrabItem)
                {
                    canGrabItem = false;
                    if (mouse.texture != green)
                        mouse.texture = green;
                }
            }
        }

    }

    private void GetFurniture(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;


        if (canGrabItem)
        {
            focusedObject = null;

            grabDistance = Vector3.Distance(Camera.main.transform.position, hit.point);
            objectInHand = hit.transform.gameObject;
            ActionManager.grab.Invoke(objectInHand);

            rbObject = objectInHand.GetComponent<Rigidbody>();
            rbObject.useGravity = false;
            objectNetwork = objectInHand.GetComponent<NetworkObject>();
            mouse.texture = red;
            RequestOwnershipServerRpc(objectNetwork.NetworkObjectId, NetworkManager.Singleton.LocalClientId);
        }

    }

    private void LetGo(InputAction.CallbackContext ctx)
    {

        if (!IsOwner || objectInHand == null) return;

        objectNetwork.RemoveOwnership();
        rbObject.useGravity = true;
        ActionManager.release.Invoke();
        objectInHand = null;
        canGrabItem = false;
        mouse.texture = green;
    }

    private void ChangeAxis(InputAction.CallbackContext ctx)
    {
        if (lCurrentAxis.x != 0)
            lCurrentAxis = Vector3.up;
        else if (lCurrentAxis.y != 0)
            lCurrentAxis = Vector3.forward;
        else if (lCurrentAxis.z != 0)
            lCurrentAxis = Vector3.right;
    }

    private void Rotate(InputAction.CallbackContext ctx)
    {
        if (objectInHand == null) return;

        Vector2 lAngle = ctx.ReadValue<Vector2>();

        Vector3 lFinalAngle = lCurrentAxis * lAngle.y * 1000 * Time.deltaTime;

        lFinalAngle = new Vector3(lCurrentAxis.x * lFinalAngle.x, lCurrentAxis.y * lFinalAngle.y, lCurrentAxis.z * lFinalAngle.z);
        objectInHand.transform.Rotate(lFinalAngle, Space.Self);
    }

    private void MoveFurniture()
    {
        Ray raycastToMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 pos = raycastToMouse.origin + raycastToMouse.direction * grabDistance;

        rbObject.MovePosition(Vector3.Lerp(objectInHand.transform.position, pos, Time.deltaTime * SmoothMovementFourniture));
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong objectId, ulong requestingClientId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject netObj))
        {
            netObj.ChangeOwnership(requestingClientId);
        }
    }


}
