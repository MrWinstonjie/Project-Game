using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXHitbox : MonoBehaviour
{
    [SerializeField] public int damage = 25;           // Used for SingleDamage
    [SerializeField] public int dps = 5;               // Used for DamagePerSec
    [SerializeField] public float damageInterval = 1.3f; 

    public enum DamageType 
    { 
        SingleDamage, 
        DamagePerSec 
    }
    
    public DamageType currentDamageType; 

    private Dictionary<Entity, Coroutine> activeDamageRoutines = new Dictionary<Entity, Coroutine>();
    private HashSet<Entity> alreadyHitEntities = new HashSet<Entity>();

    void OnTriggerEnter2D(Collider2D other)
    {
        Entity hitEnemy = other.GetComponent<Entity>();

        if (hitEnemy != null && !other.CompareTag("Player"))
        {
            Entity attacker = transform.root.GetComponent<Entity>();
            int finalDamage = damage;

            if (attacker != null)
            {
                finalDamage = attacker.GetDamageOutput(damage);
            }

            switch (currentDamageType)
            {
                case DamageType.SingleDamage:
                    if (!alreadyHitEntities.Contains(hitEnemy))
                    {
                        hitEnemy.TakeDamage(finalDamage);
                        alreadyHitEntities.Add(hitEnemy);
                        Debug.Log("SINGLE HIT DAMAGED: " + other.name + " for " + finalDamage);
                    }
                    break;

                case DamageType.DamagePerSec:
                    if (!activeDamageRoutines.ContainsKey(hitEnemy))
                    {
                        Coroutine routine = StartCoroutine(DamageOverTime(hitEnemy, other.name, finalDamage));
                        activeDamageRoutines.Add(hitEnemy, routine);
                    }
                    break;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Entity hitEnemy = other.GetComponent<Entity>();

        if (hitEnemy != null)
        {
            if (activeDamageRoutines.ContainsKey(hitEnemy))
            {
                StopCoroutine(activeDamageRoutines[hitEnemy]);
                activeDamageRoutines.Remove(hitEnemy);
            }

            if (alreadyHitEntities.Contains(hitEnemy))
            {
                alreadyHitEntities.Remove(hitEnemy);
            }
        }
    }

    IEnumerator DamageOverTime(Entity enemy, string enemyName, int damagePerTick)
    {
        while (true) 
        {
            enemy.TakeDamage(damagePerTick); 
            Debug.Log("DPS HITBOX DAMAGED: " + enemyName + " for " + damagePerTick);
            
            yield return new WaitForSeconds(damageInterval); 
        }
    }
}