using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVFXHitbox : MonoBehaviour
{
    [SerializeField] public int damage = 25;
    [SerializeField] public int dps = 5;
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
        Entity hitEntity = other.GetComponent<Entity>();

        if (hitEntity != null && !other.CompareTag("Enemies"))
        {
            switch (currentDamageType)
            {
                case DamageType.SingleDamage:
                    if (!alreadyHitEntities.Contains(hitEntity))
                    {
                        hitEntity.TakeDamage(damage);
                        alreadyHitEntities.Add(hitEntity);
                    }
                    break;

                case DamageType.DamagePerSec:
                    if (!activeDamageRoutines.ContainsKey(hitEntity))
                    {
                        Coroutine routine = StartCoroutine(DamageOverTime(hitEntity));
                        activeDamageRoutines.Add(hitEntity, routine);
                    }
                    break;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Entity hitEntity = other.GetComponent<Entity>();

        if (hitEntity != null)
        {
            if (activeDamageRoutines.ContainsKey(hitEntity))
            {
                StopCoroutine(activeDamageRoutines[hitEntity]);
                activeDamageRoutines.Remove(hitEntity);
            }

            if (alreadyHitEntities.Contains(hitEntity))
            {
                alreadyHitEntities.Remove(hitEntity);
            }
        }
    }

    IEnumerator DamageOverTime(Entity target)
    {
        while (true)
        {
            target.TakeDamage(dps);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
