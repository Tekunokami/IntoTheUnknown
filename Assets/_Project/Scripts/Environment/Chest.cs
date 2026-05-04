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

        // Check if already opened in previous save
        if (GameManager.Instance != null && GameManager.Instance.currentSaveData.clearedEventIDs.Contains(chestID))
        {
            isOpened = true;
            if (animator != null) animator.Play("ChestOpen_Idle");
        }
    }

    // IInteractable Contract Methods

    public void Interact()
    {
        if (isOpened) return; // Do nothing if already opened

        isOpened = true;
        
        HidePrompt(); // Hide prompt

        // Play Animation
        if (animator != null) animator.SetTrigger("Open");

        // Give Loot
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentSaveData.coins += coinDropAmount;
            Debug.Log($"Looted {coinDropAmount} coins! Total: {GameManager.Instance.currentSaveData.coins}");

            foreach (ItemData item in itemDrops)
            {
                GameManager.Instance.currentSaveData.inventoryItemIDs.Add(item.itemID);
                Debug.Log($"Looted item: {item.itemName}!");
            }

            GameManager.Instance.currentSaveData.clearedEventIDs.Add(chestID);
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