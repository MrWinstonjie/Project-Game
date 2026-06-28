using UnityEngine;

public class Potion : MonoBehaviour
{
    public enum PotionType
    {
        Health,
        Mana,
        Defense,
        DamageIncrease
    }

    public PotionType potionType;
    public int healthAmount = 20; 
    public int manaAmount = 10;
    public int defenseAmount = 5;
    public float damageMultiplier = 1.25f;
    public float effectDuration = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Entity playerEntity = collision.GetComponent<Entity>();
            if (playerEntity != null)
            {
                switch (potionType)
                {
                    case PotionType.Health:
                        playerEntity.AddCurrentHealth(healthAmount);
                        break;
                    case PotionType.Mana:
                        playerEntity.setCurrentMana(Mathf.Min(playerEntity.CurrentMana + manaAmount, playerEntity.MaxMana));
                        break;
                    case PotionType.Defense:
                        playerEntity.ApplyDefenseBoost(defenseAmount, effectDuration);
                        break;
                    case PotionType.DamageIncrease:
                        playerEntity.ApplyDamageBoost(damageMultiplier, effectDuration);
                        break;
                }
                Destroy(gameObject);
            }
        }
    }


}
