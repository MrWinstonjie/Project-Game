using UnityEngine;
using System.Collections;

public class VFXHealHitbox : MonoBehaviour
{
    public int HealRate = 5;

    void OnTriggerEnter2D(Collider2D other)
    {
        Entity hitEntity = other.GetComponent<Entity>();

        if (hitEntity != null && other.CompareTag("Player"))
        {
            StartCoroutine(HealOverTime(hitEntity));
        }
    }

    IEnumerator HealOverTime(Entity hitEntity)
    {
        while (hitEntity.CurrentHealth < hitEntity.MaxHealth)
        {
            hitEntity.AddCurrentHealth(HealRate);
            // Debug.Log("Zone is Healing: " + hitEntity.name + " for " + HealRate + " health.");
            yield return new WaitForSeconds(0.7f); 
        }
    }


}
