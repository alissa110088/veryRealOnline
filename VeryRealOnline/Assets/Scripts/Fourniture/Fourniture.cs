using UnityEngine;

public class Fourniture : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

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
        rb.useGravity = false;

    }

    private void Release()
    {
        rb.useGravity = true;
    }
}
