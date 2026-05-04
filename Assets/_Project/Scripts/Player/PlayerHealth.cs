using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStats baseStats;
    public float currentHealth;
    private bool isDead = false;

    void Start()
    {
        if (baseStats != null)
            currentHealth = baseStats.maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, baseStats.maxHealth);
        
        if(UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, baseStats.maxHealth);
            UIManager.Instance.UpdateStatsDisplay();
        }

        Debug.Log("You Lost! Remaining Health: " + currentHealth);

        if (currentHealth <= 0) Die();


        
    }

    void Die()
    {
        isDead = true;
        Debug.Log("You Died...Restarting Level.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}