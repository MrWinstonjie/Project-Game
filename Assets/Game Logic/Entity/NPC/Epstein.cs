using UnityEngine;

public class Epstein : Entity
{

    public float speed = 2f;
    public Transform pointA;
    public Transform pointB;
    private Transform target;
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        print("Epstein has Spawned");
        target = pointB;
        
    }

    // Update is called once per frame
    void Update()
    {
        //  Move();
    }

    // void Move()
    // {
    //     float move = 0f;

    //     int r = Random.Range(0, 2);

    //         if (r == 0)
    //         {
    //             target = pointA;
    //         }
    //         else
    //         {
    //             target = pointB;
    //         }

        
    //     if (target == pointB)
    //     {
    //         move = speed;

            
    //     }
    //     else if(target == pointA)
    //     {
    //         move = -speed;

            
    //     }

    //     // apply movement
    //     rb.linearVelocity = new Vector2(move, rb.linearVelocity.y);

    // }



}
