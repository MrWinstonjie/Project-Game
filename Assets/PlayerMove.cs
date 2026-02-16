using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class cha : MonoBehaviour
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


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        float move = 0f;
        

        // // Jump
        
         if(rb.linearVelocity.y < 0  && Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            jumpCount+=2;

        }else if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            jumpCount++;
            
        }
        
        // double jump
        // if(!grounded && Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        // {
        //     rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
        //     jumpCount+=2;
        // }


        // jumping anim
        if(rb.linearVelocity.y > 5){
            anim.SetBool("Jump",true);
            print("jumping");
        }



        // Horizontal movement
        if (Input.GetKey(KeyCode.D))
        {
            move = moveSpeed;
            anim.SetInteger("AnimState", 1);

            if (!facingRight)
                Flip();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            move = -moveSpeed;
            anim.SetInteger("AnimState", 1);
            if (facingRight)
                Flip();
        }
        else
        {
            anim.SetInteger("AnimState", 0);
        }

            
            
        rb.linearVelocity = new Vector2(move, rb.linearVelocity.y);
        


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


// if touching the floor
    void OnCollisionEnter2D(Collision2D collision)
    {
        print("is touching the floor");
        grounded = true;
        jumpCount = 0;
    }
// if in the air

    void OnCollisionExit2D(Collision2D collision)
    {
        print("is in the air");
        grounded = false;
        
    }
    

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}
