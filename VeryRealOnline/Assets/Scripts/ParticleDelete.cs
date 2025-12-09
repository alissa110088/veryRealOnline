using UnityEngine;

public class ParticleDelete : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;

    void Update()
    {
        if (!particleSystem.isPlaying)
        {
            Destroy(gameObject);
        }        
    }
}
