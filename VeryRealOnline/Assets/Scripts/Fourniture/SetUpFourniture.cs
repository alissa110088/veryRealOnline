using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class SetUpFourniture : MonoBehaviour
{
    private int layerInt = 6;
    private void Awake()
    {
        gameObject.layer = layerInt;

        Destroy(this);
    }
}
