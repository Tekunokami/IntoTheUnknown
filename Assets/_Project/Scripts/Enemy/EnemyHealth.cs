using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyData data;
    public Animator animator;
    
    private float currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = data.maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            // If health is 0 or less, trigger Death
            Die();
        }
        else
        {
            // If he survived the hit, trigger Hurt
            animator.SetTrigger("Hurt");
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        
        // Disable movement and collisions
        GetComponent<EnemyController>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        // Destroy after 3 seconds
        Destroy(gameObject, 3f);
    }
}