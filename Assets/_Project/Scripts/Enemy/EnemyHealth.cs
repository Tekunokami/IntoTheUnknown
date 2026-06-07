using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyData data;
    public Animator animator;
    private float currentHealth;
    private bool isDead = false;
    private int scaledCoinValue;

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

        int progress = 0;
        if (GameManager.Instance != null && GameManager.Instance.currentSaveData != null)
        {
            progress = GameManager.Instance.currentSaveData.roomsClearedCount;
        }

        float hpMultiplier = 1f + (progress * 0.10f);
        currentHealth = data.maxHealth * hpMultiplier;
        
        scaledCoinValue = Mathf.RoundToInt(data.coinValue * (1f + (progress * 0.05f)));
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
        
        if (GameManager.Instance != null && GameManager.Instance.currentSaveData != null)
        {
            // Dynamically scale coin rewards
            int progress = GameManager.Instance.currentSaveData.roomsClearedCount;
            int finalCoinValue = Mathf.RoundToInt(data.coinValue * (1f + (progress * 0.05f)));
            
            GameManager.Instance.currentSaveData.coins += finalCoinValue;

            GameManager.Instance.currentSaveData.enemiesKilledCount++;
            
            if (!GameManager.Instance.currentSaveData.clearedEventIDs.Contains(uniqueEnemyID))
            {
                GameManager.Instance.currentSaveData.clearedEventIDs.Add(uniqueEnemyID);
            }

            if (UIManager.Instance != null) UIManager.Instance.UpdateCoinDisplay();
        Destroy(gameObject, 3f);
        }
    
    }
}