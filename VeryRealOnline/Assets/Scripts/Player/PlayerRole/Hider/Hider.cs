using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hider : NetworkBehaviour
{
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private float distanceToGrab = 5f;
    [SerializeField] private float SmoothMovementFourniture = 15f;
    [SerializeField] private float howCloseUISpawn = .6f;
    [SerializeField] private float howHighUiSpawn = .5f;

    private InputSystem_Actions inputActions;
    private GameObject objectInHand = null;
    private float grabDistance;
    private bool canGrabItem;
    private bool callOneTimeUi;
    private RaycastHit hit;
    private Rigidbody rbObject;
    private NetworkObject objectNetwork;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        inputActions = new InputSystem_Actions();

        inputActions.Player.Interact.started += GetFurniture;
        inputActions.Player.Interact.canceled += (ctx) => LetGo();
        inputActions.Player.Enable();
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
                Vector3 lPosition = hit.point - (Camera.main.transform.forward * howCloseUISpawn - transform.up * howHighUiSpawn);
                ActionManager.spawnUi.Invoke(hit.transform.gameObject, lPosition, Camera.main);

                if (!callOneTimeUi)
                {
                    callOneTimeUi = true;
                }
                canGrabItem = true;
            }
            else
            {
                canGrabItem = false;
                callOneTimeUi = true;
                ActionManager.despawnUi.Invoke();
            }
        }

    }

    private void GetFurniture(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;

        if (canGrabItem)
        {
            ActionManager.grab.Invoke();
            ActionManager.despawnUi.Invoke();

            grabDistance = Vector3.Distance(Camera.main.transform.position, hit.point);
            objectInHand = hit.transform.gameObject;
            rbObject = objectInHand.GetComponent<Rigidbody>();
            objectNetwork = objectInHand.GetComponent<NetworkObject>();
            RequestOwnershipServerRpc(objectNetwork.NetworkObjectId, NetworkManager.Singleton.LocalClientId);


        }

    }

    private void LetGo()
    {
        if (!IsOwner && objectInHand == null) return;

        objectNetwork.RemoveOwnership();
        rbObject.useGravity = true;
        ActionManager.release.Invoke();
        objectInHand = null;
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
