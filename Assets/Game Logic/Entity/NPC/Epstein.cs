using UnityEngine;
using Pathfinding;
using System;

public class Epstein : Entity
{
    private Transform player;
    protected float activationRange = 5f;
    private AIDestinationSetter pathScript;
    private bool isChasing = false; 
    private Rigidbody2D rb;
    private bool facingRight = true;
    private Animator anim;
    private bool isAttacking1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pathScript = GetComponent<AIDestinationSetter>();
        pathScript.enabled = false; 
        anim = GetComponent<Animator>();

        GameObject p = GameObject.Find("Player");
        if (p != null){
            player = p.transform;
            // print("Player found");
        }
        else
            Debug.LogError("No Player object found in the scene!");

        
    }
    


    void Update()
    {
        CheckDeath();
        if(IsDead) return;
        
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (!isChasing && distance <= activationRange)
        {
            pathScript.enabled = true;
            isChasing = true;
        }
        else if (isChasing && distance > activationRange)
        {
            pathScript.enabled = false;
            isChasing = false;
        }

        if (player.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && facingRight)
        {
            Flip();
        }

      


    }

    void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.CompareTag("Player"))
        {
            Attack();
            print("NPC Attacked The Player");
        }
        
    }



    void Flip()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void Attack()
    {
        isAttacking1 = true;
        anim.SetBool("isAttacking1", true);
        anim.Play("Attack1");
 
    }

    void AttackDelay()
    {
        isAttacking1 = false;
    }

    public void Idle()
    {
        isAttacking1 = false;
    }

    void CheckDeath()
    {
        if(IsDead)
        {
           Destroy(gameObject, 1.5f); 
        }
    }


}