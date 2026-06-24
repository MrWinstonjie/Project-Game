using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Skeletor : Entity
{
    private Rigidbody2D rb;
    private Vector2 movement;
    private int startDirection = 1; 
    private float speed = 3f;
    private int currentDirection;
    private float halfWidth;
    private float jumpForce = 17f;
    private bool grounded = true;
    float turnTimer;
    float nextTurnTime;
    int nextMove;
    private Animator anim;
    private bool facingRight = true;
    private bool isHurt = false;
    private bool isAttacking1; // Used this to block spamming
    private bool isIdling = false;
    private Coroutine activeFreeze;

    void Start()
    {
        currentDirection = startDirection;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        halfWidth = GetComponent<BoxCollider2D>().bounds.extents.x;
        nextTurnTime = Random.Range(2f, 5f);
        nextMove = Random.Range(0,5);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        print("SKELETOR HAS TAKEN DAMAGE");
        Hurt();
    }

    void Update()
    {
        if (!isIdling && !isAttacking1 && !isHurt) 
        {
            // PRIORITY 1: Check for the player first
            checkObstacle();

            // If checkObstacle triggered an attack, 'isAttacking1' is now true.
            // We use 'return;' to instantly stop reading code so he doesn't try to walk or idle!
            if (isAttacking1) return;


            // PRIORITY 2: Roll the dice to see if he should take a break
            wander();

            // If wander triggered an idle, stop reading!
            if (isIdling) return;


            // PRIORITY 3: If he didn't attack, and didn't idle, THEN he is allowed to walk.
            movPatrol(); 

            // Apply the actual movement physics AFTER we know he's definitely walking
            movement.x = speed * currentDirection;
            movement.y = rb.linearVelocity.y;
            rb.linearVelocity = movement;
        }
        else 
        {
            // STOP moving horizontally, but keep vertical velocity for gravity
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void wander()
    {
        turnTimer += Time.deltaTime;

        if (turnTimer >= nextTurnTime) 
        {
            nextMove = Random.Range(0, 5);

            if (nextMove == 1)
            {
                StartCoroutine(Idle()); 
            }
            else
            {
                currentDirection *= -1;
                Flip();
                
                turnTimer = 0f;
                nextTurnTime = Random.Range(2f, 5f);
            }
        }
    }



    private void handleJump()
    {
        if (grounded)
        {
            grounded = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        grounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
    }

    private void checkObstacle()
    {
        Vector2 bottomOrigin = (Vector2)transform.position + Vector2.down * 0.2f;
        Vector2 middleOrigin = (Vector2)transform.position;
        Vector2 topOrigin = (Vector2)transform.position + Vector2.up * 1.3f;
        
        if (!grounded) return;

        Vector2 dir = currentDirection > 0 ? Vector2.right : Vector2.left;
        float distance = halfWidth + 0.3f;

        bool bottomHit = Physics2D.Raycast(bottomOrigin, dir, distance, LayerMask.GetMask("Map","Obstacle"));
        bool middleHit = Physics2D.Raycast(middleOrigin, dir, distance, LayerMask.GetMask("Map","Obstacle"));
        bool topHit = Physics2D.Raycast(topOrigin, dir, distance, LayerMask.GetMask("Map","Obstacle"));

        bool bottomHitPlayer = Physics2D.Raycast(bottomOrigin, dir, distance, LayerMask.GetMask("Player"));
        bool middleHitPlayer = Physics2D.Raycast(middleOrigin, dir, distance, LayerMask.GetMask("Player"));

        if ((bottomHit || middleHit) && !topHit)
        {
            handleJump();
        }

       
        if ((bottomHitPlayer || middleHitPlayer) && !topHit)
        {
            if (!isAttacking1)
            {
                Attack(); 
            }
        }

        Debug.DrawRay(bottomOrigin, dir * distance, Color.red);
        Debug.DrawRay(middleOrigin, dir * distance, Color.yellow);
        Debug.DrawRay(topOrigin, dir * distance, Color.green);
    }

    private void movPatrol()
    {
        
        if (!isAttacking1){
            anim.Play("Walk");  
        }
        
        Vector2 bottomOrigin = (Vector2)transform.position + Vector2.down * 0.2f;
        Vector2 middleOrigin = (Vector2)transform.position;
        Vector2 topOrigin = (Vector2)transform.position + Vector2.up * 1.3f;

        if (grounded)
        {
            if ((Physics2D.Raycast(bottomOrigin, Vector2.right, halfWidth + 0.1f, LayerMask.GetMask("Map","Obstacle")) 
            || Physics2D.Raycast(middleOrigin, Vector2.right, halfWidth + 0.1f, LayerMask.GetMask("Map","Obstacle"))) && currentDirection > 0)
            {
                Flip();
                grounded = true;
                currentDirection = -1; 
            }
            else if ((Physics2D.Raycast(bottomOrigin, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Map","Obstacle")) 
            || Physics2D.Raycast(middleOrigin, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Map","Obstacle"))) && currentDirection < 0)
            {
                Flip();
                grounded = true;
                currentDirection = 1;
            }
        }
    }

    private void Hurt()
    {
        rb.linearVelocity = Vector2.zero; 
        if (activeFreeze != null)
        {
            StopCoroutine(activeFreeze);
        }
        anim.speed = 1f; 
        anim.Play("Hurt");
    }

    private void Attack()
    {
        isAttacking1 = true;
        anim.Play("Attack1");

        Invoke("AttackDelay", 2f); 
    }

    void AttackDelay()
    {
        isAttacking1 = false;
    }

    public void FreezeAnimation()
    {
            
        if (activeFreeze != null) StopCoroutine(activeFreeze);
            
        activeFreeze = StartCoroutine(FreezeRoutine(1f));
    }

    private IEnumerator FreezeRoutine(float pauseDuration)
     {
        anim.speed = 0f;
        yield return new WaitForSeconds(pauseDuration);
        anim.speed = 1f;
            
        activeFreeze = null; // Clear the reference when done
    }

    IEnumerator Idle()
    {
        isIdling = true;
        anim.Play("Idle");
        
        yield return new WaitForSeconds(2f);
        
        isIdling = false;
        turnTimer = 0; 
        nextTurnTime = Random.Range(2f, 5f);
    }

    void ResetHurt()
    {
        isHurt = false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}