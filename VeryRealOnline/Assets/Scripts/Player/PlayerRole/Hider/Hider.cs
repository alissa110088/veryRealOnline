using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hider : NetworkBehaviour
{
    public Camera cam;

    //[SerializeField] private LayerMask objectLayer;
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


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        objectLayer = LayerMask.GetMask("Obstacle");


        inputActions = new InputSystem_Actions();

        inputActions.Player.Interact.started += GetFurniture;
        inputActions.Player.Interact.canceled += (ctx) => LetGo();

        
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


        if (objectInHand == null)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, distanceToGrab, objectLayer))
            {
                ActionManager.spawnUi.Invoke(hit.transform.gameObject, hit.point, Camera.main);
                focusedObject = hit.transform.gameObject;
                canGrabItem = true;
            }
            else
            {
                if (focusedObject != null)
                {
                    ActionManager.despawnUi.Invoke(focusedObject);
                    canGrabItem = false;
                    return;
                }
            }
        }

    }

    private void GetFurniture(InputAction.CallbackContext ctx)
    {
        Debug.Log(IsOwner + " " + NetworkObject.NetworkObjectId.ToString());

        if (!IsOwner) return;


        if (canGrabItem)
        {
            ActionManager.grab.Invoke();
            ActionManager.despawnUi.Invoke(focusedObject);
            focusedObject = null;

            grabDistance = Vector3.Distance(Camera.main.transform.position, hit.point);
            Debug.Log(hit.transform.gameObject);
            objectInHand = hit.transform.gameObject;
            rbObject = objectInHand.GetComponent<Rigidbody>();
            objectNetwork = objectInHand.GetComponent<NetworkObject>();
            RequestOwnershipServerRpc(objectNetwork.NetworkObjectId, NetworkManager.Singleton.LocalClientId);
        }

    }

    private void LetGo()
    {
        Debug.Log(IsOwner);

        if (!IsOwner && objectInHand == null) return;

        objectNetwork.RemoveOwnership();
        rbObject.useGravity = true;
        ActionManager.release.Invoke();
        objectInHand = null;
        canGrabItem = false ;
    }

    private void MoveFurniture()
    {
        Ray raycastToMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 pos = raycastToMouse.origin + raycastToMouse.direction * grabDistance;

        rbObject.MovePosition(Vector3.Lerp(
        objectInHand.transform.position,
        pos,
        Time.deltaTime * SmoothMovementFourniture));
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
