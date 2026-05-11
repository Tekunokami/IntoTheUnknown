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
    public float yThreshold = 2f;

    // The State Machine 
    private enum EnemyState { Patrolling, Chasing, Attacking }
    private EnemyState currentState;

    private float nextAttackTime;
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = EnemyState.Patrolling;

        // Find object with tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Based on distance to player, decide on state
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool isLevelWithPlayer = Mathf.Abs(player.position.y - transform.position.y) < yThreshold; // Check if player is on same platform 

        if (distanceToPlayer <= data.attackRange && isLevelWithPlayer)
        {
            currentState = EnemyState.Attacking;
        }
        else if (distanceToPlayer <= data.detectionRange && isLevelWithPlayer)
        {
            // Only chase if player is on same platform 
            if (Mathf.Abs(player.position.y - transform.position.y) < 2f) 
            {
                currentState = EnemyState.Chasing;
            }
            else
            {
                currentState = EnemyState.Patrolling;
            }
        }
        else
        {
            currentState = EnemyState.Patrolling;
        }

        // Based on current state, perform actions
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
        }
    }

    private void Patrol() // Move forward, turn around at edges or walls
    {
        animator.SetBool("isRunning", true);

        rb.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * data.patrolSpeed, rb.linearVelocity.y);

        // Check for ground ahead (Laser pointing down)
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, detectionDistance, groundLayer);
        // Check for Walls (Laser pointing forward)
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, isFacingRight ? Vector2.right : Vector2.left, detectionDistance, groundLayer);

        // If there's no ground or there's a wall ahead, turn around
        if (groundInfo.collider == false || wallInfo.collider == true)
        {
            Flip();
        }
    }

    private void Chase() // Move towards player with faster speed
    {
        animator.SetBool("isRunning", true);

        // Determine direction to player
        float directionToPlayer = player.position.x - transform.position.x;

        // Move towards player
        rb.linearVelocity = new Vector2(Mathf.Sign(directionToPlayer) * data.chaseSpeed, rb.linearVelocity.y);

        // Face the player
        if (directionToPlayer > 0 && !isFacingRight) Flip();
        else if (directionToPlayer < 0 && isFacingRight) Flip();

        // If we lose the ground beneath us, stop moving to prevent falling
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, detectionDistance, groundLayer);
        if (groundInfo.collider == false)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("isRunning", false);
        }
    }

    private void Attack()
    {
        // Stop moving
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isRunning", false);

        // Check Cooldown
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack"); // Trigger attack
            nextAttackTime = Time.time + data.attackCooldown;
            
            // TODO: Damage is dealt via Animation Events 
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    // Draw lasers in the editor to adjust their positions
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
            Gizmos.DrawLine(wallDetection.position, wallDetection.position + (isFacingRight ? Vector3.right : Vector3.left) * detectionDistance);
        }
    }
}