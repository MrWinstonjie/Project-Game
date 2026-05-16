using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class hulk : Entity
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
    private bool isAttacking1;
    private bool isIdling = false;
    

    void Start()
    {
        currentDirection = startDirection;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        halfWidth = GetComponent<BoxCollider2D>().bounds.extents.x;
        Flip();
        nextTurnTime = Random.Range(2f, 5f);
        nextMove = Random.Range(0,5);

    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        print("HULK HAS TAKEN DAMAGE");
    }


    void Update()
    {
        
        movement.x = speed * currentDirection;
        movement.y = rb.linearVelocity.y;
        rb.linearVelocity = movement;

        if (isHurt) return;


        if (!isHurt && !isIdling) 
        {
            movement.x = speed * currentDirection;
            movement.y = rb.linearVelocity.y;
            rb.linearVelocity = movement;
            
            movPatrol(); 
            wander();
            checkObstacle();
        }
        else if (isIdling || isHurt)
        {
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
        // print("HULK touch floor");
        grounded = true;

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // print("HULK in air");
        grounded = false;

    }

    // void OnTriggerEnter2D(Collider2D other) {

    //     if (other.CompareTag("PlayerAttack")) {
            
    //         Hurt();
            
    //     }

    // }

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
            Attack(); 
        }

        Debug.DrawRay(bottomOrigin, dir * distance, Color.red);
        Debug.DrawRay(middleOrigin, dir * distance, Color.yellow);
        Debug.DrawRay(topOrigin, dir * distance, Color.green);
    }

    private void movPatrol()
    {

        anim.Play("Walk");
        
        
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
                print("FUUUUCK");
            }
            else if ((Physics2D.Raycast(bottomOrigin, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Map","Obstacle")) 
            || Physics2D.Raycast(middleOrigin, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Map","Obstacle"))) && currentDirection < 0)
            {
                Flip();
                grounded = true;
                currentDirection = 1;
                print("SHIIIT");
            }

        }

        Debug.DrawRay(bottomOrigin, Vector2.right * (halfWidth + 0.10f), Color.red);
        Debug.DrawRay(middleOrigin, Vector2.right * (halfWidth + 0.10f), Color.yellow);
        Debug.DrawRay(topOrigin, Vector2.right * (halfWidth + 0.10f), Color.green);

        Debug.DrawRay(bottomOrigin, Vector2.left * (halfWidth + 0.10f), Color.red);
        Debug.DrawRay(middleOrigin, Vector2.left * (halfWidth + 0.10f), Color.yellow);
        Debug.DrawRay(topOrigin, Vector2.left * (halfWidth + 0.10f), Color.green);

            
    }

    public void Hurt()
    {

        
        if (isHurt) return;

        isHurt = true;
        
        rb.linearVelocity = Vector2.zero; 
        
        anim.Play("Hurt");

        Invoke("ResetHurt", 0.5f); 
    }

    void Attack()
    {
        anim.Play("Attack");
        print("HULK HITS PLAYER");
    }

    void AttackDelay()
    {
        isAttacking1 = false;
        
    }

    IEnumerator Idle()
    {
        isIdling = true;
        anim.Play("idle");
        
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