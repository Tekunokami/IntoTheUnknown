using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyData data;
    public Animator animator;
    private float currentHealth;
    private bool isDead = false;

    [Header("Unique Enemy ID")]
    [Tooltip("Right-click this script in the inspector and select 'Generate ID'")]
    public string uniqueEnemyID;

    [ContextMenu("Generate Unique ID")]
    private void GenerateID() {
        uniqueEnemyID = System.Guid.NewGuid().ToString();
    }

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsEventCleared(uniqueEnemyID))
        {
            Destroy(gameObject);
            return;
        }
        currentHealth = data.maxHealth;
    }
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        
        GetComponent<EnemyController>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        // Enemy death tracked
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MarkEventCleared(uniqueEnemyID, data.coinValue);
        }

        // Update the UI
        if (UIManager.Instance != null) UIManager.Instance.UpdateCoinDisplay();

        Destroy(gameObject, 3f);
    }
}