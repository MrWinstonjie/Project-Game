using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        
    public float smoothSpeed = 0.125f; 
    public Vector3 offset;          

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, offset.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}