using System;
using System.Collections;
using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class cha : Entity
{
    public float moveSpeed;
    public float jumpHeight;
    private Rigidbody2D rb;
    private bool facingRight = true;
    private Animator anim;
    public Transform groundCheck;
    public bool grounded = false;
    private int maxJumps = 2;
    private int jumpCount = 0;
    BoxCollider2D box;
    Vector2 normalSize;
    Vector2 normalOffset;
    Vector2 rollSize = new Vector2(0.7f, 0.7f);
    Vector2 rollOffset = new Vector2(-0.2f, 0.4f);
    bool isDashing = false;
    bool isRolling = false;
    int comboStep = 0;
    float comboResetTime = 0.6f;
    Coroutine comboResetCoroutine;
    bool isAttacking = false;
    float AttackSpeed = 1f;
    public DialogueLogic dialogueLogic;



    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
        

        normalSize = box.size;
        normalOffset = box.offset;
        


    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }

        if (isRolling)
        {
            return;
        }

        HandleMove();

        HandleJump();

        StartCoroutine(HandleDash());

        StartCoroutine(HandleRoll());

        HandleIdle();

        Attack();

       
  
    }


    void HandleIdle()
    {    
        // return to idle animation
        if (grounded)
        {
            anim.SetBool("Grounded",true);
            

        }
        else
        {
            anim.SetFloat("AirSpeedY", rb.linearVelocity.y);
            anim.SetBool("Grounded",false);
            
        }

    }


    void HandleJump()
    {
        // Jump and double jump
        if(rb.linearVelocity.y < 0  && Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            jumpCount+=2;

        }else if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            jumpCount++;
            
        }

        // jumping anim
        if(rb.linearVelocity.y > 5){
            anim.SetBool("Jump",true);
            print("jumping");
        }

    }


    void HandleMove()
    {
        float move = 0f;

        // Horizontal movement
        if (Input.GetKey(KeyCode.D) )
        {
            move = moveSpeed;
            anim.SetInteger("AnimState", 1);

            if (!facingRight)
            {
                Flip();
            }
                
        }
        else if (Input.GetKey(KeyCode.A) )
        {
            move = -moveSpeed;
            anim.SetInteger("AnimState", 1);
            if (facingRight)
            {
                Flip();
            }
                
        }
        else
        {
            anim.SetInteger("AnimState", 0);
        }

        // for moving the character
        rb.linearVelocity = new Vector2(move, rb.linearVelocity.y);
        
        
        
        
    }


    IEnumerator HandleRoll()
    {
        
        // Roll
        if (Input.GetKeyDown(KeyCode.LeftShift) && anim.GetCurrentAnimatorStateInfo(0).IsName("Roll") == false && grounded && !isDashing)
        {
            anim.SetBool("Roll", true);
            isRolling = true;

            box.size = rollSize;
            box.offset = rollOffset;
            

            yield return new WaitForSeconds(0.7f);

            isRolling = false;
        }
        
    }


    // Dash
    IEnumerator HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") == false && grounded && !isRolling)
        {
            anim.SetBool("Dash", true);

            isDashing = true;
            if (facingRight)
            {
                rb.linearVelocity = new Vector2(20, 0); 
                
            }else if (!facingRight)
            {
                rb.linearVelocity = new Vector2(-20, 0);
                
            }

            yield return new WaitForSeconds(0.2f);

            isDashing = false;

        }


 
    }

    IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(comboResetTime);
        comboStep = 0;
    }


    void Attack()
    {
       

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking && !dialogueLogic.talking)
        {
            isAttacking = true;

            comboStep++;
            if (comboStep > 3)
            {
                comboStep = 1;
            }
                

            anim.Play("Attack" + comboStep);
        }
    }

    void AttackBox1()
    {
        // print("test_ATTACKBOX");
    }

    void RemoveAttackbox()
    {
        // print("Removed_AttackBox");
    }

    public void Hurt()
    {
        anim.Play("Hurt");
    }


    public void AttackDelay()
    {
        isAttacking = false;
    }


    public void EndRoll()
    {
        print("endroll");
        anim.SetBool("Roll", false);
        box.size = normalSize;
        box.offset = normalOffset;
    }

    public void EndDash()
    {
        print("end Dash");
        anim.SetBool("Dash", false);
    }



// if touching the floor
    void OnCollisionEnter2D(Collision2D collision)
    {
        // print("is touching the floor");
        grounded = true;
        jumpCount = 0;
    }

// if in the air
    void OnCollisionExit2D(Collision2D collision)
    {
        // print("is in the air");
        grounded = false;
        
    }


    // if touching enemies
    void OnTriggerEnter2D(Collider2D collision)
    {
        //// Sementara gini

        if (collision.CompareTag("Enemies"))
        {
            // Hurt();
        }
        print("Collided with entity");


        
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}
