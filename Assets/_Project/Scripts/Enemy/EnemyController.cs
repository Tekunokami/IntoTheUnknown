using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData data;

    [Header("Environmental Detection")]
    public Transform groundDetection; 
    public Transform wallDetection;  
    public float detectionDistance = 0.5f;
    public LayerMask groundLayer;
    public float stoppingDistance = 0.8f;

    [Header("References")]
    public Animator animator;
    private Rigidbody2D rb;
    private Transform player;
    private EnemyCombat combat;

    [Header("Refined Settings")]
    public float yThreshold = 2f; // To prevent attacking/chasing while player is on a different level  
    private float nextAttackTime = 0f;
    private float lastFlipTime;

    // The State Machine 
    private enum EnemyState { Patrolling, Chasing, Attacking }
    private EnemyState currentState;    
    private bool isFacingRight = true; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = EnemyState.Patrolling;
        
        combat = GetComponent<EnemyCombat>();

        if (combat == null)
        {   
            Debug.LogError("EnemyCombat script is MISSING on " + gameObject.name + "!");
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
{
    if (player == null) return;

    float distanceToPlayer = Vector2.Distance(transform.position, player.position);
    bool isLevelWithPlayer = Mathf.Abs(player.position.y - transform.position.y) < yThreshold; 

    // Change state based on distance and level alignment
    if (distanceToPlayer <= data.attackRange && isLevelWithPlayer)
    {
        currentState = EnemyState.Attacking;
    }
    else if (distanceToPlayer <= data.detectionRange && isLevelWithPlayer)
    {
        currentState = EnemyState.Chasing;
    }
    else
    {
        currentState = EnemyState.Patrolling;
    }

    // Execute the logic
    switch (currentState)
    {
        case EnemyState.Patrolling: Patrol(); break;
        case EnemyState.Chasing: Chase(); break;
        case EnemyState.Attacking: Attack(); break;
    }
}

    private void Patrol() 
    {
        animator.SetBool("isRunning", true);

        // Move forward
        rb.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * data.patrolSpeed, rb.linearVelocity.y);

        // If at an edge or wall, turn around and brake for a split second
        if (Time.time >= lastFlipTime + 0.5f) // The cooldown
        {
            if (IsAtEdge() || IsAtWall())
            {
                Flip();
                lastFlipTime = Time.time; // Reset the timer
            }
        }
    }

    private void Chase() 
{
    // Direction to player 
    float directionToPlayer = player.position.x - transform.position.x;
    float absoluteDistance = Mathf.Abs(directionToPlayer);

    // lip Logic
    if (absoluteDistance > 0.5f)
    {
        if (directionToPlayer > 0 && !isFacingRight) Flip();
        else if (directionToPlayer < 0 && isFacingRight) Flip();
    }
    
    // Check if player is in attack range 
    Collider2D playerInSlot = Physics2D.OverlapCircle(combat.attackPoint.position, combat.attackRadius, combat.playerLayer);
    
    if (playerInSlot != null && Time.time >= nextAttackTime)
    {
        // Target is in attack range, stop and attack
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isRunning", false);
        
        animator.SetTrigger("Attack");
        
        nextAttackTime = Time.time + data.attackCooldown; 
        return; 
    }

    // If we're farther than stopping distance and not at an edge/wall, keep chasing
    if (absoluteDistance > stoppingDistance && !IsAtEdge() && !IsAtWall())
    {
        animator.SetBool("isRunning", true);
        rb.linearVelocity = new Vector2(Mathf.Sign(directionToPlayer) * data.chaseSpeed, rb.linearVelocity.y);
    }
    else
    {
        // Otherwise, stop and wait for the next attack opportunity
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isRunning", false);
    }
}

    private void Attack()
    {
        // Stop moving
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isRunning", false);

        // Face the player while attacking 
        float directionToPlayer = player.position.x - transform.position.x;
        if (directionToPlayer > 0 && !isFacingRight) Flip();
        else if (directionToPlayer < 0 && isFacingRight) Flip();

        // 3. Check Cooldown
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack"); 
            nextAttackTime = Time.time + data.attackCooldown;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    // --- HELPER METHODS ---
    private bool IsAtEdge()
    {
        // Check the direction we're facing
        float xOffset = isFacingRight ? 0.5f : -0.5f;
        Vector2 checkPosition = new Vector2(transform.position.x + xOffset, transform.position.y);

        // Shoot a laser straight down
        RaycastHit2D groundInfo = Physics2D.Raycast(checkPosition, Vector2.down, 1.5f, groundLayer);

        // If the laser doesn't hit, we're on edge
        return groundInfo.collider == null; 
    }

    private bool IsAtWall() 
    {
        // Laser shoots forward, if it hits a wall
        Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(wallDetection.position, dir, detectionDistance, groundLayer);
    }
    
    private void OnDrawGizmos()
    {
        if (groundDetection != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundDetection.position, groundDetection.position + Vector3.down * detectionDistance);
        }
        if (wallDetection != null)
        {
            Gizmos.color = Color.blue;
            Vector3 dir = isFacingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallDetection.position, wallDetection.position + dir * detectionDistance);
        }
    }
}