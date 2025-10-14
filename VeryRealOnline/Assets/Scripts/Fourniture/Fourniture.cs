using UnityEngine;

public class Fourniture : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    private bool isGrabbed;

    private void OnEnable()
    {
        ActionManager.grab += OnGrabbed;
        ActionManager.release += Release;
    }
    private void OnDisable()
    {
        ActionManager.grab -= OnGrabbed;
        ActionManager.release -= Release;
    }

    private void OnGrabbed()
    {

        rb.linearVelocity = Vector3.zero;
        isGrabbed = true;
        rb.useGravity = false;

    }

    private void Release()
    {
        isGrabbed = false;
        rb.useGravity = true;
    }
}
