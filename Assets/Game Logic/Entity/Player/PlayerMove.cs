using System;
using System.Collections;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;



public class cha : Entity
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
    BoxCollider2D box;
    Vector2 normalSize;
    Vector2 normalOffset;
    Vector2 rollSize = new Vector2(0.7f, 0.7f);
    Vector2 rollOffset = new Vector2(-0.2f, 0.4f);
    bool isDashing = false;
    bool isRolling = false;
    int comboStep = 0;
    float comboResetTime = 0.6f;
    Coroutine comboResetCoroutine;
    bool isAttacking = false;
    float AttackSpeed = 1f;
    public DialogueLogic dialogueLogic;
    public GameObject UltFX;
    public GameObject UltFXInitial;
    public GameObject FireSlashVfx;
    public GameObject SlamVfx;
    public GameObject HealthSystems;
    public Transform SpawnPoint;
    private bool dead;
    public int DeathCounter;
    public int FuryCounter;
    public bool Invincible;
    [Header("Death Recovery")]
    public GameObject lastDeathPickupPrefab;
    private LastDeath activeLastDeathPickup;

    public enum InventoryPotionType
    {
        Health,
        Mana,
        Rage,
        Defense
    }

    [Header("Inventory")]
    public int healPotionQty;
    public int manaPotionQty;
    public int ragePotionQty;
    public int defensePotionQty;
    public int coinCount;
    public int healPotionAmount = 20;
    public int manaPotionAmount = 10;
    public int defenseAmount = 5;
    public float rageDuration = 5f;
    public float rageMultiplier = 1.25f;
    public float defenseDuration = 5f;
    public TextMeshProUGUI HealPotionQty;
    public TextMeshProUGUI ManaPotionQty;
    public TextMeshProUGUI RagePotionQty;
    public TextMeshProUGUI DefensePotionQty;
    public int Coins;
    public TextMeshProUGUI CoinQty;


    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
        HealthSystems = GameObject.Find("HealthSystem");



        dead = false;
        setMaxHealth(150);
        setMaxMana(150);

        setCurrentHealth(80);
        setCurrentMana(MaxMana);

        HealthSystem.Instance.healthText.text = "Health: " + CurrentHealth + "/" + MaxHealth;
        HealthSystem.Instance.manaText.text = "Mana: " + CurrentMana + "/" + MaxMana;

        HealthSystem.Instance.maxHitPoint = MaxHealth;
        HealthSystem.Instance.hitPoint = MaxHealth;
        HealthSystem.Instance.maxManaPoint = MaxMana;
        HealthSystem.Instance.manaPoint = MaxMana;
        
        normalSize = box.size;
        normalOffset = box.offset;

        UpdateInventoryUI();

    }

    public override void Die()
    {
        if (dead)
        {
            return;
        }

        dead = true;
        anim.Play("Death");
        CreateLastDeathPickup();
        ClearInventoryOnDeath();
        StartCoroutine(Respawn());
    }

    public override void TakeDamage(int damage)
    {
        Debug.Log("Enemy Damaged for " + damage + " damage");
        if (IsDead)
        {
            return;
        }

        if (!Invincible)
        {
            int finalDamage = ApplyDefense(damage);
            setCurrentHealth(CurrentHealth - finalDamage);
        }
        else
        {
            return;
        }

        if (CurrentHealth <= 0){
            Die();
        }
    }



    void CreateLastDeathPickup()
    {
        if (activeLastDeathPickup != null)
        {
            Destroy(activeLastDeathPickup.gameObject);
            activeLastDeathPickup = null;
        }

        GameObject pickupObject = lastDeathPickupPrefab != null
            ? Instantiate(lastDeathPickupPrefab, transform.position, Quaternion.identity)
            : new GameObject("LastDeathPickup");

        pickupObject.transform.position = transform.position;

        LastDeath lastDeath = pickupObject.GetComponent<LastDeath>();
        if (lastDeath == null)
        {
            lastDeath = pickupObject.AddComponent<LastDeath>();
        }

        lastDeath.Configure(this, healPotionQty, manaPotionQty, ragePotionQty, defensePotionQty, coinCount);
        activeLastDeathPickup = lastDeath;

        if (pickupObject.GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = pickupObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
        }

        if (pickupObject.GetComponent<SpriteRenderer>() == null)
        {
            SpriteRenderer renderer = pickupObject.AddComponent<SpriteRenderer>();
            renderer.color = Color.magenta;
        }
    }

    void ClearInventoryOnDeath()
    {
        healPotionQty = 0;
        manaPotionQty = 0;
        ragePotionQty = 0;
        defensePotionQty = 0;
        coinCount = 0;
        UpdateInventoryUI();
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2f);
        transform.position = SpawnPoint.position;
        setCurrentHealth(MaxHealth);
        setCurrentMana(MaxMana);
        HealthSystem.Instance.hitPoint = CurrentHealth;
        HealthSystem.Instance.manaPoint = CurrentMana;
        dead = false;
        anim.Play("Idle");
    }

    void Update()
    {
        HandlePotionInput();
        CheckStats();
        

        HealthSystem.Instance.hitPoint = CurrentHealth;
        HealthSystem.Instance.manaPoint = CurrentMana;

        HealthSystem.Instance.healthText.text = "Health: " + CurrentHealth + "/" + MaxHealth;
        HealthSystem.Instance.manaText.text = "Mana: " + CurrentMana + "/" + MaxMana;

        HealthSystem.Instance.UpdateHealthBar();
        HealthSystem.Instance.UpdateManaBar();


        if (dead)
        {
            rb.linearVelocity = Vector2.zero;
        }    



        if (isDashing || isRolling || dead)
        {
            return;
        }

        HandleMove();
        HandleJump();

        StartCoroutine(HandleDash());
        StartCoroutine(HandleRoll());
        StartCoroutine(HandleAbility());

        HandleIdle();
        Attack();


  
    }

    void HandlePotionInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && healPotionQty > 0)
        {
            UsePotion(InventoryPotionType.Health);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && manaPotionQty > 0)
        {
            UsePotion(InventoryPotionType.Mana);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && ragePotionQty > 0)
        {
            UsePotion(InventoryPotionType.Rage);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && defensePotionQty > 0)
        {
            UsePotion(InventoryPotionType.Defense);
        }
    }

    public void AddCoins(int amount)
    {
        coinCount += amount;
        UpdateInventoryUI();
    }

    public bool TrySpendCoins(int amount)
    {
        if (coinCount <= 0 || coinCount < amount)
        {
            return false;
        }

        coinCount -= amount;
        UpdateInventoryUI();
        return true;
    }

    public bool TryBuyPotion(InventoryPotionType potionType, int cost = 1)
    {
        if (!TrySpendCoins(cost))
        {
            return false;
        }

        AddPotion(potionType);
        return true;
    }

    public void AddPotion(InventoryPotionType potionType, int amount = 1)
    {
        switch (potionType)
        {
            case InventoryPotionType.Health:
                healPotionQty += amount;
                break;
            case InventoryPotionType.Mana:
                manaPotionQty += amount;
                break;
            case InventoryPotionType.Rage:
                ragePotionQty += amount;
                break;
            case InventoryPotionType.Defense:
                defensePotionQty += amount;
                break;
        }

        UpdateInventoryUI();
    }

    void UsePotion(InventoryPotionType potionType)
    {
        switch (potionType)
        {
            case InventoryPotionType.Health:
                if (healPotionQty <= 0) return;
                healPotionQty--;
                AddCurrentHealth(healPotionAmount);
                break;
            case InventoryPotionType.Mana:
                if (manaPotionQty <= 0) return;
                manaPotionQty--;
                setCurrentMana(Mathf.Min(CurrentMana + manaPotionAmount, MaxMana));
                break;
            case InventoryPotionType.Rage:
                if (ragePotionQty <= 0) return;
                ragePotionQty--;
                ApplyDamageBoost(rageMultiplier, rageDuration);
                break;
            case InventoryPotionType.Defense:
                if (defensePotionQty <= 0) return;
                defensePotionQty--;
                ApplyDefenseBoost(defenseAmount, defenseDuration);
                break;
        }

        UpdateInventoryUI();
    }

    void UpdateInventoryUI()
    {
        if (HealPotionQty != null) HealPotionQty.text = healPotionQty.ToString();
        if (ManaPotionQty != null) ManaPotionQty.text = manaPotionQty.ToString();
        if (RagePotionQty != null) RagePotionQty.text = ragePotionQty.ToString();
        if (DefensePotionQty != null) DefensePotionQty.text = defensePotionQty.ToString();
        if (CoinQty != null) CoinQty.text = coinCount.ToString();
    }

    void CheckStats()
    {
        if(CurrentHealth > MaxHealth)
        {
            setCurrentHealth(MaxHealth);
        }

        if(CurrentMana > MaxMana)
        {
            setCurrentMana(MaxMana);
        }

        if(CurrentHealth < 0)
        {
            setCurrentHealth(0);
        }

        if(CurrentMana < 0)
        {
            setCurrentMana(0);
        }
    }


    void HandleIdle()
    {    
        // return to idle animation
        if (grounded)
        {
            
            anim.SetBool("Jump",false);
            anim.SetBool("Grounded",true);
        }
        else
        {
            anim.SetFloat("AirSpeedY", rb.linearVelocity.y);
            anim.SetBool("Grounded",false);
            
        }

    }


    void HandleJump()
    {

        // Jump and double jump
        if(rb.linearVelocity.y < 0  && Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            anim.SetBool("Grounded", false);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            jumpCount+=2;

        }else if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            jumpCount++;
            
        }
        

        // jumping anim
        if(rb.linearVelocity.y > 5){
            anim.SetBool("Jump",true);
            // print("jumping");
        }

    }


    void HandleMove()
    {
        float move = 0f;

        // Horizontal movement
        if (Input.GetKey(KeyCode.D) )
        {
            move = moveSpeed;
            anim.SetInteger("AnimState", 1);

            if (!facingRight)
            {
                Flip();
            }
                
        }
        else if (Input.GetKey(KeyCode.A) )
        {
            move = -moveSpeed;
            anim.SetInteger("AnimState", 1);
            if (facingRight)
            {
                Flip();
            }
                
        }
        else
        {
            anim.SetInteger("AnimState", 0);
        }

        // for moving the character
        rb.linearVelocity = new Vector2(move, rb.linearVelocity.y);
        
        
        
        
    }


    IEnumerator HandleRoll()
    {
        
        // Roll
        if (Input.GetKeyDown(KeyCode.LeftShift) && anim.GetCurrentAnimatorStateInfo(0).IsName("Roll") == false && grounded && !isDashing)
        {
            anim.SetBool("Roll", true);
            isRolling = true;
            

            box.size = rollSize;
            box.offset = rollOffset;
            

            yield return new WaitForSeconds(0.7f);

            isRolling = false;
        }
        
    }


    // Dash
    IEnumerator HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") == false && grounded && !isRolling)
        {
            anim.SetBool("Dash", true);

            isDashing = true;
            if (facingRight)
            {
                rb.linearVelocity = new Vector2(20, 0); 
                
            }else if (!facingRight)
            {
                rb.linearVelocity = new Vector2(-20, 0);
                
            }
           
           

            yield return new WaitForSeconds(0.2f);

            isDashing = false;

        }

    }



    IEnumerator HandleAbility()
    {
        if (Input.GetKeyDown(KeyCode.Q) && anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") == false && grounded && !isRolling && CurrentMana >= 80)
        {
            DrainCurrentMana(80);
            anim.SetBool("Dash", true);

            isDashing = true;
            if (facingRight)
            {
                rb.linearVelocity = new Vector2(60, 0); 
                
            }else if (!facingRight)
            {
                rb.linearVelocity = new Vector2(-60, 0);
                
            }
           
            StartCoroutine(SpawnDashTrail());

            yield return new WaitForSeconds(0.2f);
            isDashing = false;
            anim.Play("Attack2");
        }

        if (Input.GetKeyDown(KeyCode.G) && grounded && CurrentMana >= 40)
        {
            DrainCurrentMana(40);
            anim.Play("Attack3");
            StartCoroutine(SpawnSlamAttack(transform.position));
        }



        if (Input.GetKeyDown(KeyCode.F) && CurrentMana >= 15)
        {
            DrainCurrentMana(15);
            anim.Play("Jump");
            StartCoroutine(SpawnFireSlash(transform.position));
        }
        
 
    }

    IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(comboResetTime);
        comboStep = 0;
    }

    void Attack()
    {
       
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking && !dialogueLogic.talking)
        {
            isAttacking = true;

            comboStep++;
            if (comboStep > 3)
            {
                comboStep = 1;
            }

            anim.Play("Attack" + comboStep);
        }
        else
        {
            isAttacking = false;
        }
    }



    void AttackBox1()
    {
        // print("test_ATTACKBOX");
    }

    void RemoveAttackbox()
    {
        // print("Removed_AttackBox");
    }

    public void Hurt()
    {
        anim.Play("Hurt");

    }


    public void AttackDelay()
    {
        isAttacking = false;
    }


    public void EndRoll()
    {
        // print("endroll");
        anim.SetBool("Roll", false);
        box.size = normalSize;
        box.offset = normalOffset;
    }

    public void EndDash()
    {
        // print("end Dash");
        anim.SetBool("Dash", false);
    }

    IEnumerator SpawnFireSlash(Vector3 pos)
    {
        anim.Play("Jump");
        rb.linearVelocity = new Vector2(0, 15); 
        if (FireSlashVfx == null) yield break; 

        GameObject spawnedVfx1 = Instantiate(FireSlashVfx, pos, Quaternion.identity);
    
        Destroy(spawnedVfx1, 2f);
  
    }

    IEnumerator SpawnSlamAttack(Vector3 pos)
    { 
            if (SlamVfx == null) yield break; 
            Vector3 point1 = transform.position;

            yield return new WaitForSeconds(0.3f);
            GameObject spawnedVfx1 = Instantiate(SlamVfx, point1, Quaternion.identity);

            if (transform.localScale.x < 0) 
            {
                spawnedVfx1.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else{
                spawnedVfx1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
                    
            Destroy(spawnedVfx1, 2f);
  
    }

    IEnumerator SpawnDashTrail()
    {
    if (UltFX == null) yield break;

        Vector3 offset = new Vector3(0, 1.5f, 0); 

        Vector3 point1 = transform.position + offset;
        yield return new WaitForSeconds(0.1f);

        Vector3 point2 = transform.position + offset;
        yield return new WaitForSeconds(0.1f);

        Vector3 point3 = transform.position + offset;
        yield return new WaitForSeconds(0.1f);

        GameObject vfx0 = Instantiate(UltFXInitial, point2, Quaternion.identity);
        yield return new WaitForSeconds(0.6f);

        GameObject vfx1 = Instantiate(UltFX, point1, Quaternion.identity);
        GameObject vfx2 = Instantiate(UltFX, point2, Quaternion.identity);
        GameObject vfx3 = Instantiate(UltFX, point3, Quaternion.identity);

        Destroy(vfx0, 2f);
        Destroy(vfx1, 2f);
        Destroy(vfx2, 2f);
        Destroy(vfx3, 2f);
    }



    // if touching the floor
    void OnCollisionEnter2D(Collision2D collision)
    {
        // print("is touching the floor");
        grounded = true;
        jumpCount = 0;
    }

    // if in the air
    void OnCollisionExit2D(Collision2D collision)
    {
        // print("is in the air");
        grounded = false;
        
    }


    // if touching enemies
    void OnTriggerEnter2D(Collider2D collision)
    {
        //// Sementara gini

        if (collision.CompareTag("Enemies"))
        {
            // Hurt();
        }
        // print("Collided with entity");


        
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}
