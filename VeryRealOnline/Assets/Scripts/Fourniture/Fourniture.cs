using UnityEngine;

public class Fourniture : MonoBehaviour
{
    public Rigidbody rb;

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

    private void OnGrabbed(GameObject pObject)
    {
        if (gameObject != pObject)
            return;
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;
    }

    private void Release()
    {
        rb.useGravity = true;
    }
}
