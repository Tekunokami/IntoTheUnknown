using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public EnemyData data;
    public Transform attackPoint;
    public float attackRadius = 0.5f;
    public LayerMask playerLayer;

    public void ExecuteAttack() 
    {
        // Detect player in range of attack
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        // Damage the player
        if (hitPlayer != null)
        {
            if (hitPlayer.TryGetComponent(out PlayerHealth pHealth))
            {
                // Apply damage to player and knockback
                pHealth.TakeDamage(data.attackDamage, transform);
                
                Debug.Log("Enemy hit the player!");
            }
        }
    }

    // Visualize the attack range in the editor
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}