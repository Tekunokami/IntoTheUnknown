using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStats baseStats;
    private float currentHealth;
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