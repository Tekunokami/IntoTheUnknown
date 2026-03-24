using UnityEngine;
using UnityEngine.InputSystem; 
using System.Collections;


public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private GameControls controls; 
    private bool isGrounded;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 13.5f;
    

    [Header("Game Feel")]
    public float coyoteTime = 0.2f; 
    private float coyoteCounter;
    public float jumpBufferTime = 0.2f; 
    private float jumpBufferCounter;


    [Header("Detection")]
    public Transform groundCheck; 
    public float groundCheckRadius = 0.05f; 
    public Transform ceilingCheck; 
    public Vector2 ceilingBoxSize = new Vector2(0.8f, 0.1f);
    public LayerMask groundLayer; 


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
        rb = GetComponent<Rigidbody2D>();

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
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        bool isTouchingCeiling = Physics2D.OverlapBox(ceilingCheck.position, ceilingBoxSize, 0f, groundLayer);

        if (isTouchingCeiling && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }

        if (isGrounded) coyoteCounter = coyoteTime;
        else coyoteCounter -= Time.deltaTime;

        moveInput = controls.Player.Move.ReadValue<Vector2>();

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

        if (isDashing && Time.time - lastGhostSpawnTime >= ghostSpawnDelay)
        {
            SpawnGhost();
            lastGhostSpawnTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }
   
    private void ExecuteJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpBufferCounter = 0;
        coyoteCounter = 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (ceilingCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(ceilingCheck.position, ceilingBoxSize);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RoomExit"))
        {
            RoomManager.Instance.SpawnNewRoom();
        }
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