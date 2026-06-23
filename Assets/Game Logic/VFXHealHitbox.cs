using UnityEngine;
using System.Collections;

public class VFXHealHitbox : MonoBehaviour
{
    public int HealRate = 5;
    
    private Coroutine activeHealCoroutine;

    void OnTriggerEnter2D(Collider2D other)
    {
        Entity hitEntity = other.GetComponent<Entity>();

        if (hitEntity != null && other.CompareTag("Player"))
        {
            if (activeHealCoroutine != null)
            {
                StopCoroutine(activeHealCoroutine);
            }
            
            activeHealCoroutine = StartCoroutine(HealOverTime(hitEntity));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (activeHealCoroutine != null)
            {
                StopCoroutine(activeHealCoroutine);
                activeHealCoroutine = null; 
            }
        }
    }

    IEnumerator HealOverTime(Entity hitEntity)
    {
        while (hitEntity.CurrentHealth < hitEntity.MaxHealth)
        {
            hitEntity.AddCurrentHealth(HealRate);
            yield return new WaitForSeconds(0.7f); 
        }
        
        activeHealCoroutine = null;
    }
}