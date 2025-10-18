using UnityEngine;
using UnityEngine.UI;

public class InteractionUIScript : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GameObject furniture;
    [SerializeField] private float howCloseUISpawn = .6f;
    [SerializeField] private float howHighUiSpawn = .5f;

    private Camera camera;
    private bool shouldFollowPlayer;

    private void OnEnable()
    {
        ActionManager.spawnUi += SpawnImage;
        ActionManager.despawnUi += DespawnImage;
    }

    private void LateUpdate()
    {
        if (shouldFollowPlayer)
        {
            Quaternion rotation = camera.transform.rotation;
            transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
            Vector3 directionToCamera = (camera.transform.position - transform.position).normalized;
            Vector3 lPosition = transform.position + directionToCamera * howCloseUISpawn + transform.up * howHighUiSpawn;

            transform.position = lPosition;
        }
    }

    private void SpawnImage(GameObject pFurniture, Vector3 SpawnPoint, Camera cam)
    {
        if (furniture != pFurniture)
            return;

        if (!image.gameObject.activeInHierarchy)
            image.gameObject.SetActive(true);

        transform.position = SpawnPoint;
        camera = cam;
        shouldFollowPlayer = true;
    }

    private void DespawnImage(GameObject focusedObject)
    {
        if (furniture != focusedObject)
            return;

        image.gameObject.SetActive(false);
        shouldFollowPlayer = false;
    }

    private void OnDisable()
    {
        ActionManager.spawnUi -= SpawnImage;
        ActionManager.despawnUi -= DespawnImage;
    }
}
