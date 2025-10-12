using UnityEngine;
using UnityEngine.UI;

public class InteractionUIScript : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GameObject Furniture;

    private Camera camera;
    private bool shouldFollowPlayer;

    private void OnEnable()
    {
        ActionManager.spawnUi += SpawnImage;
        ActionManager.despawnUi += DespawnImage;
    }

    void Start()
    {
    }

    private void LateUpdate()
    {
        if (shouldFollowPlayer)
        {
            Quaternion rotation = camera.transform.rotation;
            transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
            Vector3 directionToCamera = (camera.transform.position - transform.position).normalized;
            Vector3 lPosition = transform.position + directionToCamera * .3f + transform.up * .5f;

            transform.position = lPosition; 

        }
    }

    private void SpawnImage(GameObject pFurniture, Vector3 SpawnPoint, Camera cam)
    {
        if(Furniture != pFurniture)
            return;

        image.enabled = true;
        transform.position = SpawnPoint;
        camera = cam;
        shouldFollowPlayer = true;
    }

    private void DespawnImage()
    {
        image.enabled = false;
        shouldFollowPlayer = false;
    }

    private void OnDisable()
    {
        ActionManager.spawnUi -= SpawnImage;
        ActionManager.despawnUi -= DespawnImage;
    }
}
