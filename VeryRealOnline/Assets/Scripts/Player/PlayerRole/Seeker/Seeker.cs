using UnityEngine;

public class Seeker : MonoBehaviour
{
    //[SerializeField] private LayerMask playerLayer;

    private LayerMask playerLayer;
    private GameObject focusedObject;

    private void OnEnable()
    {
        playerLayer = LayerMask.GetMask("Player");
    }
    private void Update()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * 5f, Color.red, 0.1f);

        if (Physics.Raycast(transform.position, transform.forward, out hit, 5f, playerLayer))
        {

            ActionManager.spawnUi.Invoke(hit.transform.gameObject, hit.point, Camera.main);
            focusedObject = hit.transform.gameObject;
            Debug.Log("found Player");
        }
        else
        {
            if (focusedObject != null)
            {
                ActionManager.despawnUi.Invoke(focusedObject);
                focusedObject = null;
            }
        }
    }
}
