using UnityEngine;

public class MerchantZone : MonoBehaviour
{
    public enum MerchantPotionType
    {
        Health,
        Mana,
        Rage,
        Defense
    }

    public GameObject interactPromptUI;
    private bool isPlayerInRange = false;
    private cha player;

    [Header("Merchant Settings")]
    public MerchantPotionType potionForSale = MerchantPotionType.Health;

    [Header("Potion Prices")]
    public int healthPotionCost = 1;
    public int manaPotionCost = 1;
    public int ragePotionCost = 2;
    public int defensePotionCost = 2;

    private void Update()
    {
        if (!isPlayerInRange || player == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            cha.InventoryPotionType potionType = ConvertPotionType(potionForSale);
            int cost = GetPotionCost(potionForSale);

            if (player.coinCount <= 0)
            {
                Debug.Log("Not enough coins");
                return;
            }

            if (player.TryBuyPotion(potionType, cost))
            {
                Debug.Log("Bought " + potionForSale.ToString().ToLower() + " potion");
            }
            else
            {
                Debug.Log("Not enough coins");
            }
        }
    }

    private cha.InventoryPotionType ConvertPotionType(MerchantPotionType type)
    {
        switch (type)
        {
            case MerchantPotionType.Mana:
                return cha.InventoryPotionType.Mana;
            case MerchantPotionType.Rage:
                return cha.InventoryPotionType.Rage;
            case MerchantPotionType.Defense:
                return cha.InventoryPotionType.Defense;
            default:
                return cha.InventoryPotionType.Health;
        }
    }

    private int GetPotionCost(MerchantPotionType type)
    {
        switch (type)
        {
            case MerchantPotionType.Mana:
                return manaPotionCost;
            case MerchantPotionType.Rage:
                return ragePotionCost;
            case MerchantPotionType.Defense:
                return defensePotionCost;
            default:
                return healthPotionCost;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<cha>();
            isPlayerInRange = true;
            if (interactPromptUI != null) interactPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null;
            if (interactPromptUI != null) interactPromptUI.SetActive(false);
        }
    }
}
