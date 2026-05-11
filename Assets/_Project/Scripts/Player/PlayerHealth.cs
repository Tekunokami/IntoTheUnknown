using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStats baseStats;
    public float currentHealth;
    private bool isDead = false;

    [Header("References")]
    public Animator animator;
    public PlayerController playerController;

    void Start()
    {
        if (baseStats != null) currentHealth = baseStats.maxHealth;
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

        if (currentHealth <= 0) 
        {
            Die();
        }
        else 
        {
            // Triggers Hurt Animation
            if (animator != null) animator.SetTrigger("Player_Damaged");
        }

        
    }

    void Die()
    {
        isDead = true;
        if (playerController != null) playerController.isDead = true; // Lock movement
        Debug.Log("DIE FUNCTION CALLED - Sending 'Health_Zero' to Animator");
        if (animator != null) 
        {
            animator.SetTrigger("Health_Zero");
        }

        else
        {
            Debug.LogError("ANIMATOR MISSING ON PLAYERHEALTH SCRIPT!");
        }
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // Wait for death animation to finish
        yield return new WaitForSeconds(3f); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}