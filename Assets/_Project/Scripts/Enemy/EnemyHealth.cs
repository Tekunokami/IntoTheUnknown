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
        if (string.IsNullOrEmpty(uniqueEnemyID))
        {
            Debug.LogError($"<color=red>WAIT!</color> The enemy '{gameObject.name}' is missing a Unique ID! It will not save correctly.");
        }

        if (GameManager.Instance != null && GameManager.Instance.IsEventCleared(uniqueEnemyID))
        {
            // This enemy is already dead in the save data, so we destroy it 
            Destroy(gameObject);
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

        // Give loot directly to save data
        if (GameManager.Instance != null && GameManager.Instance.currentSaveData != null)
        {
            // Add coins
            GameManager.Instance.currentSaveData.coins += data.coinValue;
            
            // Remember that this specific enemy is dead
            if (!GameManager.Instance.currentSaveData.clearedEventIDs.Contains(uniqueEnemyID))
            {
                GameManager.Instance.currentSaveData.clearedEventIDs.Add(uniqueEnemyID);
            }
        }
        // Update the UI
        if (UIManager.Instance != null) UIManager.Instance.UpdateCoinDisplay();

        Destroy(gameObject, 3f);
    }
}