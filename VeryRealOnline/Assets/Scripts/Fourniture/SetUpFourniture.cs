using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class SetUpFourniture : MonoBehaviour
{
    private int layerInt = 6;
    private int massRB = 10;
    private NetworkRigidbody nRigidBody;
    private NetworkTransform nTranform;
    private Rigidbody rb;
    private Fourniture furniture;
    private void Awake()
    {
        //gameObject.AddComponent<NetworkObject>();

        //nRigidBody = gameObject.AddComponent<NetworkRigidbody>();
        //nRigidBody.AutoSetKinematicOnDespawn = false;
        //nRigidBody.AutoUpdateKinematicState = false;
        //nRigidBody.UseRigidBodyForMotion = true;

        //nTranform = gameObject.GetComponent<NetworkTransform>();
        //nTranform.SyncScaleX = false;
        //nTranform.SyncScaleY = false;
        //nTranform.SyncScaleZ = false;
        //nTranform.AuthorityMode = NetworkTransform.AuthorityModes.Owner;

        //rb = gameObject.GetComponent<Rigidbody>();
        //rb.freezeRotation = true;
        //rb.isKinematic = false;
        //rb.mass = massRB;

        //furniture = gameObject.AddComponent<Fourniture>();
        //furniture.rb = rb;

        gameObject.layer = layerInt;

        Destroy(this);
    }

    //private void OnDestroy()
    //{
    //    Destroy(nRigidBody);
    //    Destroy(nTranform);
    //    Destroy(rb);
    //    Destroy(furniture);
    //}
}
