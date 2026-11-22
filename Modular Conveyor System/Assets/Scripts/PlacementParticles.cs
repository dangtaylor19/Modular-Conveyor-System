using UnityEngine;

public class PlacementParticles : MonoBehaviour
{
    public ParticleSystem canParticles;
    public ParticleSystem boxParticles;
    
    public void TriggerParticles(Vector3 worldPos){
        transform.position = worldPos;

        canParticles.Play();
        boxParticles.Play();
    }
}
