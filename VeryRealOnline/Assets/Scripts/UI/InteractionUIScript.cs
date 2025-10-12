using UnityEngine;
using UnityEngine.UI;

public class InteractionUIScript : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GameObject Furniture;

    private Camera camera;

    private void OnEnable()
    {
        ActionManager.spawnUi += SpawnImage;
    }

    void Start()
    {
    }

    private void LateUpdate()
    {
        camera = Camera.main;

        Quaternion rotation = camera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    private void SpawnImage(GameObject pFurniture, Vector3 SpawnPoint)
    {
        if(Furniture != pFurniture)
            return;

        image.enabled = true;
        transform.position = SpawnPoint;
    }
}
