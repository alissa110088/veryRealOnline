using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Hider : NetworkBehaviour
{

    [SerializeField] private Texture green;
    [SerializeField] private Texture darkGreen;
    [SerializeField] private Texture red;
    [SerializeField] private RawImage mouse;
    [SerializeField] private LayerMask layerToHit;

    public Camera cam;
    private float distanceToGrab = 5f;
    private float SmoothMovementFourniture = 15f;
    private InputSystem_Actions inputActions;
    private GameObject objectInHand = null;
    private float grabDistance;
    private bool canGrabItem;
    private RaycastHit hit;
    private Rigidbody rbObject;
    private NetworkObject objectNetwork;
    private Vector3 lCurrentAxis = new Vector3(0, 0, 1);
    private float rotateAngleAdd = 1000;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

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

        CheckCanGrab();
    }

    private void CheckCanGrab()
    {
        Vector3 lOrigin = cam.transform.position;
        Vector3 lDirection = cam.transform.forward;

        if (objectInHand == null)
        {
            if (Physics.Raycast(lOrigin, lDirection, out hit, distanceToGrab, layerToHit))
            {
                canGrabItem = true;
                mouse.texture = darkGreen;
            }
            else if (canGrabItem)
            {
                canGrabItem = false;
                mouse.texture = green;
            }
        }
    }

    private void GetFurniture(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;


        if (canGrabItem)
        {
            grabDistance = Vector3.Distance(Camera.main.transform.position, hit.point);
            objectInHand = hit.transform.gameObject;
            ActionManager.grab.Invoke(objectInHand);

            rbObject = objectInHand.GetComponent<Rigidbody>();
            rbObject.useGravity = false;
            objectNetwork = objectInHand.GetComponent<NetworkObject>();
            mouse.texture = red;

            RemoveGravityRPC(objectNetwork.NetworkObjectId);
            RequestOwnershipServerRpc(objectNetwork.NetworkObjectId, NetworkManager.Singleton.LocalClientId);
        }
    }

    [Rpc(SendTo.Everyone)]

    private void RemoveGravityRPC(ulong objectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject netObj))
        {
            Rigidbody rb = netObj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.useGravity = false;
        }
    }

    private void LetGo(InputAction.CallbackContext ctx)
    {

        if (!IsOwner || objectInHand == null) return;

        objectNetwork.RemoveOwnership();
        rbObject.useGravity = true;
        AddGravityRPC(objectNetwork.NetworkObjectId);
        ActionManager.release.Invoke();
        objectInHand = null;
        canGrabItem = false;
        mouse.texture = green;
    }

    [Rpc(SendTo.Everyone)]

    private void AddGravityRPC(ulong objectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject netObj))
        {
            Rigidbody rb = netObj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.useGravity = true;
        }
    }

    private void ChangeAxis(InputAction.CallbackContext ctx)
    {
        if (lCurrentAxis.x != 0f)
            lCurrentAxis = Vector3.up;
        else if (lCurrentAxis.y != 0f)
            lCurrentAxis = Vector3.forward;
        else if (lCurrentAxis.z != 0f)
            lCurrentAxis = Vector3.right;
    }

    private void Rotate(InputAction.CallbackContext ctx)
    {
        if (objectInHand == null) return;

        Vector2 lAngle = ctx.ReadValue<Vector2>();

        Vector3 lFinalAngle = lCurrentAxis * lAngle.y * rotateAngleAdd * Time.deltaTime;

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
