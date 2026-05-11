using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    public PlayerStats baseStats;
    public Animator animator;
    public PlayerController playerController;

    [Header("State")]
    public float currentHealth;
    private bool isDead = false;
    
    [Header("Combat Feel")]
    private bool isInvincible = false;
    public float invincibilityDuration = 0.5f;

    void Start()
    {
        if (baseStats != null) 
        {
            currentHealth = baseStats.maxHealth;
            UpdateUI();
        }
    }

    public void TakeDamage(float amount)
    {
        // Ignore damage if dead or invincible
        if (isDead || isInvincible) return;

        // apply defense and ensure at least 1 damage is taken
        float actualDamage = Mathf.Max(amount - baseStats.defense, 1f);

        currentHealth -= actualDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, baseStats.maxHealth);
        
        UpdateUI();

        Debug.Log("Took " + actualDamage + " damage! Remaining Health: " + currentHealth);

        if (currentHealth <= 0) 
        {
            Die();
        }
        else 
        {
            // Triggers Hurt Animation
            if (animator != null) animator.SetTrigger("Player_Damaged");
            
            // Start the brief invincibility period
            StartCoroutine(InvincibilityFrames());
        }
    }

    private IEnumerator InvincibilityFrames() // Makes the player temporarily invincible after taking damage
    {
        isInvincible = true;
                
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
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

        // Disable physical body so enemies don't keep hitting player
        if (TryGetComponent(out Collider2D coll)) coll.enabled = false;
        if (TryGetComponent(out Rigidbody2D rb)) rb.simulated = false;

        StartCoroutine(DeathRoutine());
    }

    private void UpdateUI()
    {
        // Helper function to keep code clean
        if(UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, baseStats.maxHealth);
            UIManager.Instance.UpdateStatsDisplay();
        }
    }

    private IEnumerator DeathRoutine()
    {
        // Wait for death animation to finish
        yield return new WaitForSeconds(3f); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}