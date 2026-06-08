using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [Header("Chest Settings")]
    [Tooltip("MUST be completely unique! Ex: room_1_chest_left")]
    public string chestID; 
    
    [Header("Loot Settings")]
    public int coinDropAmount;

    [Tooltip("Number of items to drop")]
    public int numberOfItems = 3;

    [Header("References")]
    public GameObject interactPrompt; 
    public Animator animator;         

    private bool isOpened = false;

    private void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);

        // Check if already opened in save/session data
        if (GameManager.Instance != null && GameManager.Instance.IsEventCleared(chestID))
        {
            isOpened = true;
            if (animator != null) animator.Play("ChestOpen_Idle");
        }
    }

    // IInteractable Contract Methods

    public void Interact()
    {
        if (isOpened) return;
        isOpened = true;
        HidePrompt(); 
        if (animator != null) animator.SetTrigger("Open");

        if (GameManager.Instance != null)
        {   
            int progress = GameManager.Instance.currentSaveData.roomsClearedCount;

            //Formula for scaling coin drops:
            float coinMultiplier = 1f + (progress * 0.15f);
            int finalCoins = Mathf.RoundToInt(coinDropAmount * coinMultiplier);

            // Give loot directly to save data
            GameManager.Instance.currentSaveData.coins += finalCoins;
            GameManager.Instance.currentSaveData.totalCoinsLooted += finalCoins;
            GameManager.Instance.currentSaveData.clearedEventIDs.Add(chestID);

            if (LootManager.Instance != null)
            {
                List<ItemData> randomLoot = LootManager.Instance.GenerateChestLoot(numberOfItems);
                
                foreach (ItemData item in randomLoot)
                {
                    if (item != null)
                    {
                        GameManager.Instance.currentSaveData.inventoryItemIDs.Add(item.itemID);
                        Debug.Log($"<color=#FFD700>RNG Loot Output: {item.itemName} (Rarity: {item.rarity})</color>");
                    }
                }
            }
            else
            {
                Debug.LogError("hest couldn't find LootManager!");
            }

            if (UIManager.Instance != null) UIManager.Instance.UpdateCoinDisplay();
        }
    }
    public void ShowPrompt()
    {
        if (!isOpened && interactPrompt != null) interactPrompt.SetActive(true);
    }

    public void HidePrompt()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }
}