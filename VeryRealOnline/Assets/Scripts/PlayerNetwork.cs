using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    NetworkVariable<int> randomNumber = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    NetworkVariable<PlayerData> playerData = new(
        new PlayerData
        {
            life = 100,
            stunt = false,
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode upKey = KeyCode.W;
    [SerializeField] private KeyCode downKey = KeyCode.S;
    [SerializeField] private string message;

    private Vector3 direction;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        playerData.OnValueChanged += (PlayerData previousValue, PlayerData newValue) =>
        {
            Debug.Log(OwnerClientId + " life" + newValue.life + " stunt" + newValue.stunt + " message " + newValue.message);
        };

        //randomNumber.OnValueChanged += (int previousValue, int newValue) => { Debug.Log(OwnerClientId + " Random Number " + randomNumber.Value); };
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        direction = Vector3.zero;

        if (Input.GetKey(leftKey))
        {
            direction.x = -1f;
        }
        if (Input.GetKey(rightKey))
        {
            direction.x = 1f;
        }
        if (Input.GetKey(downKey))
        {
            direction.z = -1f;
        }
        if (Input.GetKey(upKey))
        {
            direction.z = 1f;
        }

        direction = direction.normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //randomNumber.Value = Random.Range(0, 100);

            //playerData.Value = new PlayerData()
            //{
            //    life = Random.Range(0, 100),
            //    stunt = playerData.Value.stunt,
            //    message = "Praise the sun!"
            //};


            TestRpc(new RpcParams());
        }
    }


    [Rpc(SendTo.NotServer)]
    void TestRpc(RpcParams rpcParams)
    {
        Debug.Log("TestRpc " + OwnerClientId + "rpc Params: " + rpcParams.Receive.SenderClientId);
    }

}

public struct PlayerData : INetworkSerializable
{
    public int life;
    public bool stunt;
    public FixedString128Bytes message;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref life);
        serializer.SerializeValue(ref stunt);
        serializer.SerializeValue(ref message);
    }
}
