using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int maxHealth = 100;
    protected int currentHealth = 100;
    protected bool isDead = false;
    protected String Type = "none";

        
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }



    public virtual void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    protected virtual void Die()
    {
        isDead = true;
    }




    
}
