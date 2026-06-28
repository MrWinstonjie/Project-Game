using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private float punchForce = 15f;
    [SerializeField] private float uppercutForce = 25f;
    [SerializeField] private Transform playerRoot;
    private Collider2D swordCollider;
    
    

    private void Awake()
    {
        playerRoot = transform.root;
        swordCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        

        Debug.Log(playerRoot.name + "JUST Attacked: " + other.gameObject.name);
        Entity hitEnemy = other.GetComponent<Entity>();
        if (hitEnemy != null)
        {
            int finalDamage = attackDamage;
            Entity attacker = playerRoot.GetComponent<Entity>();
            if (attacker != null)
            {
                finalDamage = attacker.GetDamageOutput(attackDamage);
            }

            hitEnemy.TakeDamage(finalDamage);
            Debug.Log(playerRoot.name + " JUST TOFUCKEDUCHED: " + other.gameObject.name);
            Debug.Log("Enemy Health: " + hitEnemy.CurrentHealth);
        }

        KnockbackReceiver2D target = other.GetComponent<KnockbackReceiver2D>();
        
        if (target != null) 
        {
            Vector2 pushDirection = (other.transform.position - playerRoot.position).normalized;
            pushDirection = new Vector2(other.transform.position.x, 0).normalized;

            target.ApplyKnockback(pushDirection, punchForce);
        }
    }

}