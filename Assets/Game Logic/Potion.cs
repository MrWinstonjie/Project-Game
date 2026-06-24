using UnityEngine;

public class Potion : MonoBehaviour
{
    public enum PotionType
    {
        Health,
        Mana
    }

    public PotionType potionType;
    public int healthAmount = 20; 
    public int manaAmount = 10; 

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
                        playerEntity.setCurrentMana(playerEntity.CurrentMana + manaAmount);
                        break;
                }
                Destroy(gameObject);
            }
        }
    }


}
