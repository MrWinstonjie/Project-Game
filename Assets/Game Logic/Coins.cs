using UnityEngine;

public class Coins : MonoBehaviour
{
    public int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cha player = collision.GetComponent<cha>();
            if (player != null)
            {
                player.AddCoins(coinValue);
                Destroy(gameObject);
            }
        }
    }
}
