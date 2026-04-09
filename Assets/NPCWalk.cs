using UnityEngine;

public class NPCWalk : MonoBehaviour // Pastikan nama NPCWalk sama dengan nama file
{
    public float speed = 2f;
    public float walkDistance = 3f;
    
    private Vector3 startPosition;
    private bool movingRight = true;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float rightBoundary = startPosition.x + walkDistance;
        float leftBoundary = startPosition.x - walkDistance;

        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            spriteRenderer.flipX = false; 

            if (transform.position.x >= rightBoundary)
                movingRight = false;
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            spriteRenderer.flipX = true; 

            if (transform.position.x <= leftBoundary)
                movingRight = true;
        }
    }
}