using UnityEngine;
using UnityEngine.UIElements;

public class hulk : MonoBehaviour
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
    

    void Start()
    {
        currentDirection = startDirection;
        rb = GetComponent<Rigidbody2D>();

        halfWidth = GetComponent<BoxCollider2D>().bounds.extents.x;
        
        nextTurnTime = Random.Range(2f, 5f);

    }


    void Update()
    {
        
        movement.x = speed * currentDirection;
        movement.y = rb.linearVelocity.y;
        rb.linearVelocity = movement;

        movPatrol(); 
        wander();
        checkObstacle();
       
    }

    private void wander()
    {
        turnTimer += Time.deltaTime;

        if (turnTimer >= nextTurnTime)
        {
            currentDirection *= -1;

            turnTimer = 0f;
            nextTurnTime = Random.Range(2f, 5f);
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
        print("HULK touch floor");
        grounded = true;

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        print("HULK in air");
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

        if ((bottomHit || middleHit) && !topHit)
        {
            handleJump();
        }

        Debug.DrawRay(bottomOrigin, dir * distance, Color.red);
        Debug.DrawRay(middleOrigin, dir * distance, Color.yellow);
        Debug.DrawRay(topOrigin, dir * distance, Color.green);
    }

    private void movPatrol()
    {
        Vector2 bottomOrigin = (Vector2)transform.position + Vector2.down * 0.2f;
        Vector2 middleOrigin = (Vector2)transform.position;
        Vector2 topOrigin = (Vector2)transform.position + Vector2.up * 1.3f;

        if (grounded)
        {

            if ((Physics2D.Raycast(bottomOrigin, Vector2.right, halfWidth + 0.1f, LayerMask.GetMask("Map","Obstacle")) 
            || Physics2D.Raycast(middleOrigin, Vector2.right, halfWidth + 0.1f, LayerMask.GetMask("Map","Obstacle"))) && currentDirection > 0)
            {
                grounded = true;
                currentDirection = -1;
                print("FUUUUCK");
            }
            else if ((Physics2D.Raycast(bottomOrigin, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Map","Obstacle")) 
            || Physics2D.Raycast(middleOrigin, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Map","Obstacle"))) && currentDirection < 0)
            {
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

}