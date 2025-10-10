using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    NetworkVariable<int> randomNumber = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //NetworkVariable<PlayerData> playerData = new(
    //    new PlayerData
    //    {
    //        life = 100,
    //        stunt = false,
    //    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] private float moveSpeed = 10f;


    private Vector3 direction;
    private InputSystem_Actions inputActions;
    private Vector3 inputDirection;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        //playerData.OnValueChanged += (PlayerData previousValue, PlayerData newValue) =>
        //{
        //    Debug.Log(OwnerClientId + " life" + newValue.life + " stunt" + newValue.stunt + " message " + newValue.message);
        //};

        inputActions = new InputSystem_Actions();
        inputActions.Player.Move.performed += GetDirection;
        inputActions.Player.Move.canceled += ctx =>  inputDirection = Vector3.zero;
        inputActions.Player.Enable();
        Cursor.visible = false;


        //randomNumber.OnValueChanged += (int previousValue, int newValue) => { Debug.Log(OwnerClientId + " Random Number " + randomNumber.Value); };
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //randomNumber.Value = Random.Range(0, 100);

            //playerData.Value = new PlayerData()
            //{
            //    life = Random.Range(0, 100),
            //    stunt = playerData.Value.stunt,
            //    message = "Praise the sun!"
            //};


            //TestRpc(new RpcParams());
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        direction = transform.forward * inputDirection.z + transform.right * inputDirection.x;
        Debug.Log(direction * moveSpeed);
        transform.position += direction * moveSpeed *Time.fixedDeltaTime;
    }

    private void GetDirection(InputAction.CallbackContext ctx)
    {
        if(!IsOwner)
            return;

        inputDirection = ctx.ReadValue<Vector2>();
        inputDirection = new Vector3(inputDirection.x, 0f, inputDirection.y);
        inputDirection = inputDirection.normalized;
    }

    //[Rpc(SendTo.NotServer)]
    //void TestRpc(RpcParams rpcParams)
    //{
    //    Debug.Log("TestRpc " + OwnerClientId + "rpc Params: " + rpcParams.Receive.SenderClientId);
    //}

}

//public struct PlayerData : INetworkSerializable
//{
//    public int life;
//    public bool stunt;
//    public FixedString128Bytes message;

//    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
//    {
//        serializer.SerializeValue(ref life);
//        serializer.SerializeValue(ref stunt);
//        serializer.SerializeValue(ref message);
//    }
//}
