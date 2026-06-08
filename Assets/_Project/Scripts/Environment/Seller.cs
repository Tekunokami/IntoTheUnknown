using System.Collections.Generic;
using UnityEngine;

public class Seller : MonoBehaviour, IInteractable
{
    [Header("Seller Identity")]
    [Tooltip("MUST be unique for each seller!")]
    public string sellerID;

    [Header("References")]
    public GameObject interactPrompt;

    private void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);

        // Create a inventory for this seller.
        GenerateInventoryIfNeeded();
    }

    private void GenerateInventoryIfNeeded()
    {
        if (GameManager.Instance == null || LootManager.Instance == null) return;

        SaveData save = GameManager.Instance.currentSaveData;

        // Check if this seller already has an inventory in save data
        SellerData myData = save.sellerInventories.Find(v => v.sellerID == sellerID);

        // RNG inventory generation 
        if (myData == null)
        {
            myData = new SellerData();
            myData.sellerID = sellerID;

            // Inventory always contains 2 common, 1 rare, 1 epic item
            ItemData common1 = LootManager.Instance.GetRandomItemFromPool(ItemRarity.Common);
            ItemData common2 = LootManager.Instance.GetRandomItemFromPool(ItemRarity.Common);
            ItemData rare = LootManager.Instance.GetRandomItemFromPool(ItemRarity.Rare);
            ItemData epic = LootManager.Instance.GetRandomItemFromPool(ItemRarity.Epic);

            if (common1 != null) myData.availableItemIDs.Add(common1.itemID);
            if (common2 != null) myData.availableItemIDs.Add(common2.itemID);
            if (rare != null) myData.availableItemIDs.Add(rare.itemID);
            if (epic != null) myData.availableItemIDs.Add(epic.itemID);

            save.sellerInventories.Add(myData);
            Debug.Log($"[Shop] {sellerID} inventory generated!");
        }
    }
    public void Interact()
    {
        if (GameManager.Instance == null) return;
         
        SellerData myData = GameManager.Instance.currentSaveData.sellerInventories.Find(v => v.sellerID == sellerID);
        if (myData == null) 
        {
            Debug.LogError($"[Shop] ERROR: No inventory found for seller {sellerID}!");
            return;
        }   
        
        if (ShopUI.Instance != null)
        {
            // If the shop UI is currently open, close it and return
            if (ShopUI.Instance.gameObject.activeInHierarchy)
            {
                ShopUI.Instance.CloseShop();
                return;
            }
            
            // If it's closed, open it
            Debug.Log($"[Shop] Interacting with seller: {sellerID}");
            ShopUI.Instance.OpenShop(sellerID);
        }
        else
        {
            Debug.LogError("[Shop] ShopUI is missing in the scene!");
        }
    }

    public void ShowPrompt()
    {
        if (interactPrompt != null) interactPrompt.SetActive(true);
    }

    public void HidePrompt()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }
}