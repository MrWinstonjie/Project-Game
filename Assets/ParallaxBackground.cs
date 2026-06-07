using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public GameObject cam;          
    public float parallaxEffect; 
    
    private float length;
    private float startPos;

    void Start()
    {
        startPos = transform.position.x;
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            if (spriteRenderer.drawMode == SpriteDrawMode.Tiled)
            {
                length = spriteRenderer.size.x * transform.localScale.x;
            }
            else
            {
                length = spriteRenderer.bounds.size.x;
            }
        }
    }

    void LateUpdate()
    {
        float temp = cam.transform.position.x * (1 - parallaxEffect);
        float distance = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if (temp > startPos + (length / 2)) 
        {
            startPos += length;
        }
        else if (temp < startPos - (length / 2)) 
        {
            startPos -= length;
        }
    }
}