using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 25;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("SWORD JUST TOUCHED: " + other.gameObject.name);

        Entity hitEnemy = other.GetComponent<Entity>();

        if (hitEnemy != null && !other.CompareTag("Player"))
        {
            hitEnemy.TakeDamage(attackDamage);
        }
    }
}