using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class FrostBoss : Entity
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
    public GameObject SlamVfx;
    public GameObject WarningFx;
    public Transform WarningSpawnPoint;
    public Transform AbilitySpawnPoint;
    [SerializeField] private float slamSpawnDelay = 1f;

    protected override void Start()
    {
        base.Start();
        isAttacking1= false;
        currentDirection = startDirection;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        halfWidth = GetComponent<BoxCollider2D>().bounds.extents.x;
        nextTurnTime = Random.Range(2f, 5f);
        nextMove = Random.Range(0,5);
        Flip();
    }

public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage); 
        
        if (CurrentHealth > 0) 
        {
            Hurt();
        }
    }

    public override void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        setDeath(true);
        rb.linearVelocity = Vector2.zero;
        SpawnCoinDrop();
        StopAllCoroutines(); 
        StartCoroutine(DeadBossAnim());
    }

    public IEnumerator WarningEffect()
    {
        if (WarningFx == null || WarningSpawnPoint == null)
        {
            yield break;
        }

        Vector3 warningPosition = WarningSpawnPoint.position;
        Instantiate(WarningFx, warningPosition, Quaternion.identity);

        yield return new WaitForSeconds(1f);
    }

    IEnumerator DeadBossAnim()
    {
        anim.Play("death");
        yield return new WaitForSeconds(1.3f);
        Destroy(gameObject);
    }

    void Update()
    {

        if (CurrentHealth <= 0) return; 

        if (!isIdling && !isAttacking1 && !isHurt) 
        {
        
            if (!isIdling && !isAttacking1 && !isHurt) 
            {
                checkObstacle();

                if (isAttacking1) return;
                wander();
                if (isIdling) return;
            
                movPatrol(); 

                movement.x = speed * currentDirection;
                movement.y = rb.linearVelocity.y;
                rb.linearVelocity = movement;
            }
            else 
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
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

    IEnumerator SpawnSlamAttack(Vector3 pos)
    { 
            if (SlamVfx == null) yield break; 

            yield return new WaitForSeconds(slamSpawnDelay);
            Vector3 point1 = AbilitySpawnPoint != null ? AbilitySpawnPoint.position : transform.position;

            GameObject spawnedVfx1 = Instantiate(SlamVfx, point1, Quaternion.identity);

            if (transform.localScale.x < 0) 
            {
                spawnedVfx1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else{
                spawnedVfx1.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
                    
            Destroy(spawnedVfx1, 2f);
  
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
        Vector2 bottomOrigin = (Vector2)transform.position + Vector2.down * 0.9f;
        Vector2 middleOrigin = (Vector2)transform.position * 2f;
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
            anim.Play("walk");  
        }
        
        Vector2 bottomOrigin = (Vector2)transform.position + Vector2.down * 0.9f;
        Vector2 middleOrigin = (Vector2)transform.position * 2f;
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
        isHurt = true;           
        isAttacking1 = false;    
        isIdling = false;      

        CancelInvoke("AttackDelay"); 
        rb.linearVelocity = Vector2.zero; 

        if (activeFreeze != null)
        {
            StopCoroutine(activeFreeze);
        }
        
        anim.speed = 1f; 
        anim.Play("take_hit");

        Invoke("ResetHurt", 0.5f); 
    }

    void ResetHurt()
    {
        isHurt = false;
    }

    public void Attack()
    {
        isAttacking1 = true;

        if (WarningFx != null && WarningSpawnPoint != null)
        {
            StartCoroutine(WarningEffect());
        }

        if (Random.value <= 0.4f && SlamVfx != null)
        {
            StartCoroutine(SpawnSlamAttack(transform.position));
        }

        anim.Play("1_atk");

        // Invoke("AttackDelay", 2f); 
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
            
        activeFreeze = null;
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



    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}