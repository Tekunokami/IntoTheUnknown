using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [Header("Chest Settings")]
    [Tooltip("MUST be completely unique! Ex: room_1_chest_left")]
    public string chestID; 
    
    [Header("Loot Settings")]
    public int coinDropAmount;
    public List<ItemData> itemDrops; 

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
            // Give loot directly to save data
            GameManager.Instance.currentSaveData.coins += coinDropAmount;
            GameManager.Instance.currentSaveData.clearedEventIDs.Add(chestID);

            foreach (ItemData item in itemDrops)
            {
                GameManager.Instance.currentSaveData.inventoryItemIDs.Add(item.itemID);
            }

            // 2. Update UI
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