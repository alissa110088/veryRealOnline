using UnityEngine;

public class Seeker : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    private GameObject focusedObject;

    private void Update()
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f, playerLayer))
        {
            ActionManager.spawnUi.Invoke(hit.transform.gameObject, hit.point, Camera.main);
            focusedObject = hit.transform.gameObject;
            Debug.Log("found Player");
        }
        else
        {
            //if(focusedObject != null)
            //{
            //    ActionManager.despawnUi.Invoke(focusedObject);
            //    focusedObject = null;
            //}
        }
    }
}
