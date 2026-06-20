using UnityEngine;

public class VFXHitbox : MonoBehaviour
{
    private int damage = 5;

    void OnTriggerEnter2D(Collider2D other)
    {
        Entity hitEnemy = other.GetComponent<Entity>();

        if (hitEnemy != null && !other.CompareTag("Player"))
        {
            hitEnemy.TakeDamage(damage);
            Debug.Log("MASSIVE HITBOX TOUCHED: " + other.name);
        }
    }
}