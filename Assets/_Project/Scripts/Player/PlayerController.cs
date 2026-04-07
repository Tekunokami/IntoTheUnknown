using UnityEngine;
using UnityEngine.InputSystem; 
using System.Collections;


public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private GameControls controls; 
    public PlayerStats baseStats;
    private bool isGrounded;

    [SerializeField] private Animator animator;

    [Header("Movement Settings")]
    // droped the move speed from 8f to 2f.
    public float moveSpeed = 2f;
    public float jumpForce = 13.5f;
    public float acceleration = 20f;

    [Header("Game Feel")]
    public float coyoteTime = 0.2f; 
    private float coyoteCounter;
    public float jumpBufferTime = 0.2f; 
    private float jumpBufferCounter;

    [Header("Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f; // How far below the player's feet to check for ground?  
    [SerializeField] private float ceilingCheckDistance = 0.1f; // How far above the player's head to check for ceilings?
    private BoxCollider2D coll;


    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;


    [Header("Ghost Effect Settings")]
    public GameObject ghostPrefab;      
    public float ghostSpawnDelay = 0.05f; 
    private float lastGhostSpawnTime;   
    
    void Awake()
    {
        // Initialize components and input system
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        controls = new GameControls();
        controls.Player.Dash.performed += ctx => OnDashPerformed();
    }

    void OnEnable()
    {
        controls.Player.Enable(); 
    }
    void OnDisable() => controls.Disable();

    void Update()
    {
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

        //Run Animation triger.
        
        if (moveInput != Vector2.zero) {
            animator.SetBool("isRunning", true);
        }
        else { animator.SetBool("isRunning", false); }


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
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        
        // Smooth horizontal movement with acceleration 
        if (Mathf.Abs(moveInput.x) > 0)
        {
            // Player is pressing a movement key, accelerate towards target speed
            float targetSpeed = moveInput.x * moveSpeed;
            float newXVelocity = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newXVelocity, rb.linearVelocity.y);
        }
        else
        {
            // Player is not pressing a movement key, decelerate to a stop
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
        //
        Vector3 groundPos = coll.bounds.center + Vector3.down * groundCheckDistance;
        Gizmos.DrawWireCube(groundPos, coll.bounds.size);

        Gizmos.color = Color.blue;
        //
        Vector3 ceilingPos = coll.bounds.center + Vector3.up * ceilingCheckDistance;
        Gizmos.DrawWireCube(ceilingPos, coll.bounds.size);
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

}