using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class SetUpFourniture : MonoBehaviour
{
    private int layerInt = 6;
    private void OnEnable()
    {
        gameObject.AddComponent<NetworkObject>();

        NetworkRigidbody lNRigidBody = gameObject.AddComponent<NetworkRigidbody>();
        lNRigidBody.AutoSetKinematicOnDespawn = false;
        lNRigidBody.AutoUpdateKinematicState = false;
        lNRigidBody.UseRigidBodyForMotion = true;

        NetworkTransform lNTranform = gameObject.GetComponent<NetworkTransform>();
        lNTranform.SyncScaleX = false;
        lNTranform.SyncScaleY = false;
        lNTranform.SyncScaleZ = false;
        lNTranform.AuthorityMode = NetworkTransform.AuthorityModes.Owner;

        Rigidbody lRB = gameObject.GetComponent<Rigidbody>();
        lRB.freezeRotation = true;
        lRB.isKinematic = false;

        Fourniture lFurniture = gameObject.AddComponent<Fourniture>();
        lFurniture.rb = lRB;

        gameObject.layer = layerInt;

        Destroy(this);
    }
}
