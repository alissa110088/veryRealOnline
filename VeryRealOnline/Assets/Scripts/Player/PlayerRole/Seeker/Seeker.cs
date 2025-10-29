using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class Seeker : NetworkBehaviour
{
    //[SerializeField] private LayerMask playerLayer;


    private LayerMask playerLayer;
    private GameObject focusedObject;
    private InputSystem_Actions inputActions;
    private bool canKill;

    private Spectate spectate;

    public override void OnNetworkSpawn()
    {
        inputActions = new InputSystem_Actions();

        inputActions.Player.Interact.started += OnHit;
    }

    private void OnEnable()
    {
        playerLayer = LayerMask.GetMask("Player");
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
        if (!IsOwner)
            return;

        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * 5f, Color.red, 0.1f);

        if (Physics.Raycast(transform.position, transform.forward, out hit, 5f, playerLayer))
        {
            if (!hit.transform.gameObject.CompareTag("hider"))
                return;

            ActionManager.spawnUi.Invoke(hit.transform.gameObject, hit.point, Camera.main);
            focusedObject = hit.transform.gameObject;
            canKill = true;
        }
        else
        {
            if (focusedObject != null)
            {
                ActionManager.despawnUi.Invoke(focusedObject);
                focusedObject = null;
                canKill = false;
            }
        }
    }

    private void OnHit(InputAction.CallbackContext ctx)
    {
        if (!canKill || !IsOwner ||focusedObject == null)
        return;

        NetworkObject targetNetObj = focusedObject.GetComponent<NetworkObject>();
        if (targetNetObj != null)
        {
            KillPlayerClientRpc(targetNetObj.NetworkObjectId);
        }
    }

    [Rpc(SendTo.Everyone)]

    private void KillPlayerClientRpc(ulong pTarget)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(pTarget, out NetworkObject spawnedObjects))
        {
            PlayerNetwork player = spawnedObjects.GetComponent<PlayerNetwork>();

            if (player == null) return;

            Hider hider = player.GetComponent<Hider>();
            if (hider == null) return;

            Spectate spectate = hider.cam.GetComponent<Spectate>();
            if (spectate != null)
            {
                spectate.enabled = true;
                spectate.RemoveMoveOption(pTarget);
            }
        }
    }
}
