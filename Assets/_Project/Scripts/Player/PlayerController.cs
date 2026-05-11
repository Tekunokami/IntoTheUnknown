using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private GameControls controls; 
    public PlayerStats baseStats;
    private bool isGrounded;
    private bool isMouseOverUI;

    [SerializeField] private Animator animator;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float jumpForce = 13.5f;
    public float acceleration = 20f;

    [Header("Combat Settings")]
    public float comboWindow = 1.0f; 
    public float attackRate = 0.4f;  
    private int comboStep = 0;
    private float lastAttackTime = 0f;
    private float nextAttackTime = 0f;

    [Header("Game Feel")]
    public float coyoteTime = 0.2f; 
    private float coyoteCounter;
    public float jumpBufferTime = 0.2f; 
    private float jumpBufferCounter;

    [Header("Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;  
    [SerializeField] private float ceilingCheckDistance = 0.1f; 
    private BoxCollider2D coll;

    [Header("Hit Detection")]
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public float swingRadius = 1.2f;
    public float spinRadius = 2.2f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;
    public bool isDead = false;

    [Header("Ghost Effect Settings")]
    public GameObject ghostPrefab;      
    public float ghostSpawnDelay = 0.05f; 
    private float lastGhostSpawnTime;   

    [Header("Interaction")]
    private IInteractable currentInteractable; // Tracks the object player is infront of
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        controls = new GameControls();
        controls.Player.Dash.performed += ctx => OnDashPerformed();
        
        // Listen for Interaction key [F]
        controls.Player.Interact.performed += ctx => TryInteract();
        controls.Player.Attack.performed += ctx => OnAttackInput();
    }

    void OnEnable()
    {
        controls.Player.Enable(); 
    }
    
    void OnDisable() => controls.Disable();

    void Update()
    {
        if (isDead || animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerHurtAnimation")) return;

        // Ground Check
        RaycastHit2D groundHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = groundHit.collider != null;
        
        // Ceiling Check
        RaycastHit2D ceilingHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, ceilingCheckDistance, groundLayer);
        bool isTouchingCeiling = ceilingHit.collider != null;

        if (isTouchingCeiling && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
        
        if (isGrounded) coyoteCounter = coyoteTime;
        else coyoteCounter -= Time.deltaTime;

        moveInput = controls.Player.Move.ReadValue<Vector2>();

        // Run Animation trigger
        if (moveInput != Vector2.zero) 
        {
            animator.SetBool("isRunning", true);
        }
        else 
        { 
            animator.SetBool("isRunning", false); 
        }

        // Jump Buffering
        if (controls.Player.Jump.triggered) jumpBufferCounter = jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0 && coyoteCounter > 0)
        {
            ExecuteJump();
        }

        if (controls.Player.Jump.WasReleasedThisFrame() && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            coyoteCounter = 0;
        }

        if (moveInput.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0) transform.localScale = new Vector3(-1, 1, 1);

        // Dash Ghost Effect
        if (isDashing && Time.time - lastGhostSpawnTime >= ghostSpawnDelay)
        {
            SpawnGhost();
            lastGhostSpawnTime = Time.time;
        }

        // Combo Attack Logic
        if (Time.time - lastAttackTime > comboWindow)
        {
            comboStep = 0;
        }

        // Update UI
        isMouseOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    void FixedUpdate()
    {   
       if (isDead || animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerHurtAnimation"))
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // Lock horizontal movement when dead or hurt// to prevent slide
            return; 
        }
        if (isDashing) return;
        
        // Smooth horizontal movement 
        if (Mathf.Abs(moveInput.x) > 0)
        {
            float targetSpeed = moveInput.x * moveSpeed;
            float newXVelocity = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newXVelocity, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }
   
    private void ExecuteJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpBufferCounter = 0;
        coyoteCounter = 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (coll == null) coll = GetComponent<BoxCollider2D>();
    
        Gizmos.color = Color.red;
        Vector3 groundPos = coll.bounds.center + Vector3.down * groundCheckDistance;
        Gizmos.DrawWireCube(groundPos, coll.bounds.size);

        Gizmos.color = Color.blue;
        Vector3 ceilingPos = coll.bounds.center + Vector3.up * ceilingCheckDistance;
        Gizmos.DrawWireCube(ceilingPos, coll.bounds.size);


        //For Attack Ranges
        if (attackPoint == null) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPoint.position, swingRadius);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(attackPoint.position, spinRadius);



    }

    private void OnDashPerformed()
    {
        if (canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash() 
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void SpawnGhost()
    {
        GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
    
        SpriteRenderer ghostSR = ghost.GetComponent<SpriteRenderer>();
        SpriteRenderer playerSR = GetComponent<SpriteRenderer>();
        ghostSR.sprite = playerSR.sprite;

        ghost.transform.localScale = transform.localScale;

        ghost.SetActive(true); 
    }

    private void OnAttackInput()
{
    // Prevent attacking if inventory is open
    if (Time.timeScale == 0) return;

    // Prevent attacking through UI
    if (isMouseOverUI) return;

    // Check if we can attack based on attack rate
    if (Time.time >= nextAttackTime) 
    {
        ExecuteAttackCombo();
    }
}

    private void ExecuteAttackCombo()
    {
        lastAttackTime = Time.time;
        nextAttackTime = Time.time + attackRate; 
        comboStep++;

        animator.ResetTrigger("LightAttack1");
        animator.ResetTrigger("LightAttack2");

        if (comboStep == 1)
        {
            animator.SetTrigger("LightAttack1");
        }
        else if (comboStep == 2)
        {
            animator.SetTrigger("LightAttack2");
            comboStep = 0; 
        }
    }

    // --- Interaction Logic ---
    private void TryInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if the object we collided has an IInteractable script
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            currentInteractable.ShowPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Clear the interactable if we walk away from it
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable.HidePrompt();
            currentInteractable = null;
        }
    }



    // --- Combat Hit Detection called by Animation Events ---
    public void ExecuteHit(int attackIndex)
    {
        // Determine which attack is being executed for radius
        float currentRadius = (attackIndex == 2) ? spinRadius : swingRadius;

        // Find all enemies in range of the attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, currentRadius, enemyLayers);

        // Apply damage
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out EnemyHealth health))
            {
                float finalDamage = CalculateDamage();
                health.TakeDamage(finalDamage);
            }
        }
    }

    private float CalculateDamage()
    {
        // pull base damage from PlayerStats
        float damage = baseStats.attackDamage;

        // The Crit Logic 
        if (Random.value <= baseStats.critRate)
        {
            damage *= baseStats.critDamage;
            Debug.Log("<color=yellow>CRITICAL HIT!</color> Dealt: " + damage);
        }

        return damage;
    }
}