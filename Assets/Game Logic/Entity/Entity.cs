using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private int maxMana = 100;
    [SerializeField] private int currentMana = 100;
    [SerializeField] private bool isDead = false;
    [SerializeField] private String type = "none";
    [SerializeField] private int defense = 0;
    [SerializeField] private float damageMultiplier = 1f;
    private Coroutine defenseBuffRoutine;
    private Coroutine damageBuffRoutine;
    
    [SerializeField] private Animator anim;
    [SerializeField] private bool dropCoinOnDeath = true;
    [SerializeField] private int coinDropValue = 1;
    [SerializeField] private GameObject coinPrefab;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int MaxMana => maxMana;
    public int CurrentMana => currentMana;
    public bool IsDead => isDead;
    public String Type => type;
    public int Defense => defense;
    public float DamageMultiplier => damageMultiplier;

    public void setMaxHealth(int value)
    {
        maxHealth = value;
    }

    public void setDeath(bool Death)
    {
        isDead = Death;
    }

    public void setCurrentHealth(int value)
    {
        currentHealth = value;
    }
    public void AddCurrentHealth(int value)
    {
        currentHealth += value;
    }

    public void setMaxMana(int value)
    {
        maxMana = value;
    }

    public void setCurrentMana(int value)
    {
        currentMana = value;
    }

    public void DrainCurrentMana(int value)
    {
        currentMana -= value;
    }

    public void ApplyDefenseBoost(int amount, float duration)
    {
        defense += amount;

        if (defenseBuffRoutine != null)
        {
            StopCoroutine(defenseBuffRoutine);
        }

        defenseBuffRoutine = StartCoroutine(ResetDefenseBoost(duration));
    }

    public void ApplyDamageBoost(float multiplier, float duration)
    {
        damageMultiplier = multiplier;

        if (damageBuffRoutine != null)
        {
            StopCoroutine(damageBuffRoutine);
        }

        damageBuffRoutine = StartCoroutine(ResetDamageBoost(duration));
    }

    public int GetDamageOutput(int baseDamage)
    {
        return Mathf.RoundToInt(baseDamage * damageMultiplier);
    }

    protected int ApplyDefense(int incomingDamage)
    {
        return Mathf.Max(0, incomingDamage - defense);
    }

    private IEnumerator ResetDefenseBoost(float duration)
    {
        yield return new WaitForSeconds(duration);
        defense = 0;
    }

    private IEnumerator ResetDamageBoost(float duration)
    {
        yield return new WaitForSeconds(duration);
        damageMultiplier = 1f;
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    public virtual void TakeDamage(int damage)
    {
        Debug.Log("Enemy Damaged for " + damage + " damage");
        if (isDead)
        {
            return;
        }

        int finalDamage = ApplyDefense(damage);
        currentHealth -= finalDamage;

        if (currentHealth <= 0){
            Die();
        }
    }


    public virtual void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    protected void SpawnCoinDrop()
    {
        if (!dropCoinOnDeath || CompareTag("Player"))
        {
            return;
        }

        if (coinPrefab != null)
        {
            GameObject coinDrop = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            Coins coinComponent = coinDrop.GetComponent<Coins>();
            if (coinComponent != null)
            {
                coinComponent.coinValue = coinDropValue;
            }
            return;
        }

        GameObject fallbackCoinDrop = new GameObject(name + " Coin Drop");
        fallbackCoinDrop.transform.position = transform.position;

        Coins fallbackCoinComponent = fallbackCoinDrop.AddComponent<Coins>();
        fallbackCoinComponent.coinValue = coinDropValue;

        CircleCollider2D collider = fallbackCoinDrop.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        SpriteRenderer spriteRenderer = fallbackCoinDrop.AddComponent<SpriteRenderer>();
        spriteRenderer.color = Color.yellow;


        Rigidbody2D rb = fallbackCoinDrop.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    public virtual void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        isDead = true;
        SpawnCoinDrop();
        Destroy(gameObject);
    }




    
}
