using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private int maxMana = 100;
    [SerializeField] private int currentMana = 100;
    [SerializeField] private bool isDead = false;
    [SerializeField] private String type = "none";
    
    [SerializeField] private Animator anim;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int MaxMana => maxMana;
    public int CurrentMana => currentMana;
    public bool IsDead => isDead;
    public String Type => type;

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

        currentHealth -= damage;

        if (currentHealth <= 0){
            Die();
        }
    }


    public virtual void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public virtual void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        isDead = true;
        Destroy(gameObject);
    }




    
}
