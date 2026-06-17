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
    public GameObject UltFX;
    public GameObject UltFXInitial;
    public GameObject FireSlashVfx;
    public GameObject SlamVfx;

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
        StartCoroutine(HandleAbility());

        HandleIdle();
        Attack();

       
  
    }


    void HandleIdle()
    {    
        // return to idle animation
        if (grounded)
        {
            anim.SetBool("Grounded",true);
            anim.SetBool("Jump",false);
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
            // print("jumping");
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

    IEnumerator HandleAbility()
    {
        if (Input.GetKeyDown(KeyCode.Q) && anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") == false && grounded && !isRolling)
        {
            anim.SetBool("Dash", true);

            isDashing = true;
            if (facingRight)
            {
                rb.linearVelocity = new Vector2(60, 0); 
                
            }else if (!facingRight)
            {
                rb.linearVelocity = new Vector2(-60, 0);
                
            }
           
            StartCoroutine(SpawnDashTrail());

            yield return new WaitForSeconds(0.2f);
            isDashing = false;
            anim.Play("Attack2");
        }

        if (Input.GetKeyDown(KeyCode.G) && grounded)
        {
            anim.Play("Attack3");
            StartCoroutine(SpawnSlamAttack(transform.position));
        }



        if (Input.GetKeyDown(KeyCode.F))
        {
            anim.Play("Jump");
            StartCoroutine(SpawnFireSlash(transform.position));
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
        // print("endroll");
        anim.SetBool("Roll", false);
        box.size = normalSize;
        box.offset = normalOffset;
    }

    public void EndDash()
    {
        // print("end Dash");
        anim.SetBool("Dash", false);
    }

    IEnumerator SpawnFireSlash(Vector3 pos)
    {
        anim.Play("Jump");
        rb.linearVelocity = new Vector2(0, 15); 
        if (FireSlashVfx == null) yield break; 

        GameObject spawnedVfx1 = Instantiate(FireSlashVfx, pos, Quaternion.identity);
    
        Destroy(spawnedVfx1, 2f);
  
    }

    IEnumerator SpawnSlamAttack(Vector3 pos)
    {
            
            if (SlamVfx == null) yield break; 
            Vector3 point1 = transform.position;

            yield return new WaitForSeconds(0.3f);
            GameObject spawnedVfx1 = Instantiate(SlamVfx, point1, Quaternion.identity);

            if (transform.localScale.x < 0) 
            {
                spawnedVfx1.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else{
                spawnedVfx1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
                    
            Destroy(spawnedVfx1, 2f);
  
    }

    IEnumerator SpawnDashTrail()
    {
    if (UltFX == null) yield break;

        Vector3 offset = new Vector3(0, 1.5f, 0); 

        Vector3 point1 = transform.position + offset;
        yield return new WaitForSeconds(0.1f);

        Vector3 point2 = transform.position + offset;
        yield return new WaitForSeconds(0.1f);

        Vector3 point3 = transform.position + offset;
        yield return new WaitForSeconds(0.1f);

        GameObject vfx0 = Instantiate(UltFXInitial, point2, Quaternion.identity);
        yield return new WaitForSeconds(0.6f);

        GameObject vfx1 = Instantiate(UltFX, point1, Quaternion.identity);
        GameObject vfx2 = Instantiate(UltFX, point2, Quaternion.identity);
        GameObject vfx3 = Instantiate(UltFX, point3, Quaternion.identity);

        Destroy(vfx0, 2f);
        Destroy(vfx1, 2f);
        Destroy(vfx2, 2f);
        Destroy(vfx3, 2f);
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
        // print("Collided with entity");


        
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}
