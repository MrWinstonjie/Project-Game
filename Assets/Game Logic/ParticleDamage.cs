using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    public int particleDamage = 5; 

    void OnParticleCollision(GameObject other)
    {
        Entity hitEnemy = other.GetComponent<Entity>();

        if (hitEnemy != null && !other.CompareTag("Player"))
        {
            // hitEnemy.TakeDamage(particleDamage);
            
            Debug.Log("A particle physically hit " + other.name + "!");
        }
    }
}