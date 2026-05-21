using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackReceiver2D : MonoBehaviour
{
    private Rigidbody2D rb;

    // Optional event to trigger animations or hit-stop
    public UnityEvent OnKnockbackReceived; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Launches the 2D entity.
    /// </summary>
    public void ApplyKnockback(Vector2 direction, float force)
    {
        // 1. Reset current velocity to prevent momentum stacking
        rb.linearVelocity = Vector2.zero;

        // 2. Apply the explosive force
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

        // 3. Trigger events
        OnKnockbackReceived?.Invoke();
    }
}