using UnityEngine;

public class LastDeath : MonoBehaviour
{
    private cha player;
    private bool isPlayerInRange;
    private int storedHealPotions;
    private int storedManaPotions;
    private int storedRagePotions;
    private int storedDefensePotions;
    private int storedCoins;

    public void Configure(cha playerRef, int heal, int mana, int rage, int defense, int coins)
    {
        player = playerRef;
        storedHealPotions = heal;
        storedManaPotions = mana;
        storedRagePotions = rage;
        storedDefensePotions = defense;
        storedCoins = coins;
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            RecoverInventory();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<cha>();
            isPlayerInRange = player != null;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null;
        }
    }

    private void RecoverInventory()
    {
        if (player == null)
        {
            return;
        }

        if (storedHealPotions > 0) player.AddPotion(cha.InventoryPotionType.Health, storedHealPotions);
        if (storedManaPotions > 0) player.AddPotion(cha.InventoryPotionType.Mana, storedManaPotions);
        if (storedRagePotions > 0) player.AddPotion(cha.InventoryPotionType.Rage, storedRagePotions);
        if (storedDefensePotions > 0) player.AddPotion(cha.InventoryPotionType.Defense, storedDefensePotions);
        if (storedCoins > 0) player.AddCoins(storedCoins);

        Destroy(gameObject);
    }
}
