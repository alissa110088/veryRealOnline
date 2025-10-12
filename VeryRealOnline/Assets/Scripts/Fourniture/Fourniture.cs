using UnityEngine;

public class Fourniture : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    private bool isGrabbed;

    private void OnEnable()
    {
        ActionManager.grab += OnGrabbed;
    }

    private void OnGrabbed()
    {
        if (!isGrabbed)
        {
            rb.linearVelocity = Vector3.zero;
            isGrabbed = true;
            rb.useGravity = false;
        }
        else
        {
            isGrabbed = false;
            rb.useGravity = true;
        }
    }
}
