using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hider : NetworkBehaviour
{
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private float distanceToGrab = 5f;
    [SerializeField] private float howCloseUISpawn = .3f;
    [SerializeField] private float howHighUiSpawn = .5f;
    [SerializeField] private float SmoothMovementFourniture = 15f;

    private InputSystem_Actions inputActions;
    private GameObject objectInHand = null;
    private float grabDistance;


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
        if (!IsOwner || objectInHand == null) return;

        MoveFurniture();

    }

    private void GetFurniture(InputAction.CallbackContext ctx)
    {
        if(!IsOwner) return;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, distanceToGrab, objectLayer))
        {
            Vector3 lPosition = hit.point - (Camera.main.transform.forward * howCloseUISpawn - transform.up * howHighUiSpawn);
            ActionManager.spawnUi.Invoke(hit.transform.gameObject, lPosition);
            ActionManager.grab.Invoke();

            grabDistance = Vector3.Distance(Camera.main.transform.position, hit.point);
            objectInHand = hit.transform.gameObject;
        }
    }

    private void LetGo()
    {
        if (!IsOwner || objectInHand == null) return;
        
        objectInHand = null;
        ActionManager.grab.Invoke();
    }

    private void MoveFurniture()
    {
        Ray raycastToMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 pos = raycastToMouse.origin + raycastToMouse.direction * grabDistance;

        objectInHand.transform.position = Vector3.Lerp(
          objectInHand.transform.position,
          pos,
          Time.deltaTime * SmoothMovementFourniture
      );
    }

}
