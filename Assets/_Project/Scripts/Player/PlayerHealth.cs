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
            currentHealth = GetTotalMaxHealth(); 
            UpdateUI();
        }
    }

    public void TakeDamage(float amount, Transform attacker = null)
    {
        // Ignore damage if dead or invincible
        if (isDead || isInvincible) return;

        // apply defense and ensure at least 1 damage is taken
        float actualDamage = Mathf.Max(amount - baseStats.defense, 1f);

        currentHealth -= actualDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, GetTotalMaxHealth());
        
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
            
            if (attacker != null && playerController != null) {
                playerController.ApplyKnockback(attacker);
            }

            // Start the brief invincibility period
            StartCoroutine(InvincibilityFrames());
        }
    }

    public float GetTotalMaxHealth()
    {
        float totalMaxHealth = baseStats.maxHealth; 
        
        if (GameManager.Instance != null && GameManager.Instance.currentSaveData != null)
        {
            ItemData[] allGameItems = Resources.LoadAll<ItemData>("Items");
            foreach (string equipID in GameManager.Instance.currentSaveData.equippedItemIDs)
            {
                EquipmentData equipData = System.Array.Find(allGameItems, item => item.itemID == equipID) as EquipmentData;
                if (equipData != null)
                {
                    totalMaxHealth += equipData.bonusHealth;
                }
            }
        }
        return totalMaxHealth;
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
        
        Debug.Log("Died! Restoring to latest save...");
        
        if (animator != null) 
        {
            animator.SetTrigger("Health_Zero");
        }
        else
        {
            Debug.LogError("PlayerHealth: No Animator found for death animation!");
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
            UIManager.Instance.UpdateHealth(currentHealth, GetTotalMaxHealth());
            UIManager.Instance.UpdateStatsDisplay();
        }
    }

    private IEnumerator DeathRoutine()
    {
        // Wait for death animation to finish
        yield return new WaitForSeconds(1.5f); 

        // UI Updates
        if(UIManager.Instance != null) {
            UIManager.Instance.ShowDeathScreen("You died, restoring to latest save...");
            UIManager.Instance.UpdateCoinDisplay(); 
        }

        yield return new WaitForSeconds(2.5f);

        //Wipe unsaved progress and reload last save
        if (GameManager.Instance != null) GameManager.Instance.ReloadFromLastSave();

        UnityEngine.SceneManagement.SceneManager.LoadScene("Test1Scene");

    }
}