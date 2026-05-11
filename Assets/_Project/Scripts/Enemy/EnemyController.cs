using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData data;

    [Header("Environmental Detection")]
    public Transform groundDetection; 
    public Transform wallDetection;  
    public float detectionDistance = 0.5f;
    public LayerMask groundLayer;

    [Header("References")]
    public Animator animator;
    private Rigidbody2D rb;
    private Transform player;

    [Header("Refined Settings")]
    public float yThreshold = 2f; // To prevent attacking/chasing while player is on a different level
    private float lastFlipTime;

    // The State Machine 
    private enum EnemyState { Patrolling, Chasing, Attacking }
    private EnemyState currentState;

    private float nextAttackTime;
    
    private bool isFacingRight = true; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = EnemyState.Patrolling;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Check if player is on roughly the same vertical level 
        bool isLevelWithPlayer = Mathf.Abs(player.position.y - transform.position.y) < yThreshold; 

        // Decision Logics
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
        // If there's no ground ahead or a wall in front, stop and wait 
        if (IsAtEdge() || IsAtWall())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("isRunning", false);
            return;
        }

        animator.SetBool("isRunning", true);

        // Determine direction to player
        float directionToPlayer = player.position.x - transform.position.x;

        // Move towards player
        rb.linearVelocity = new Vector2(Mathf.Sign(directionToPlayer) * data.chaseSpeed, rb.linearVelocity.y);

        // Face the player
        if (directionToPlayer > 0 && !isFacingRight) Flip();
        else if (directionToPlayer < 0 && isFacingRight) Flip();
    }

    private void Attack()
    {
        // Stop moving while attacking
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isRunning", false);

        // Check Cooldown
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack"); 
            nextAttackTime = Time.time + data.attackCooldown;
            
            // TODO: Implement damage dealt via Animation Events
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
        // Laser shoots downwards, if it hits nothing
        return !Physics2D.Raycast(groundDetection.position, Vector2.down, detectionDistance, groundLayer);
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