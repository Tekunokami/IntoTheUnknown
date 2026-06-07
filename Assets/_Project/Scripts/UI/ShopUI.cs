using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    [Header("Fixed Slot References")]
    [Tooltip("Soldaki 4 satıcı slotunu buraya sürükle")]
    public ShopSlot[] sellerSlots; 
    [Tooltip("Sağdaki 16 çanta slotunu buraya sürükle")]
    public ShopSlot[] playerSlots; 

    [Header("Item Info Panel")]
    public GameObject infoPanel; // Eşya seçilmediğinde gizlemek için
    public Image infoIcon;
    public TextMeshProUGUI infoName;
    public TextMeshProUGUI infoStats;
    public TextMeshProUGUI infoPrice;
    
    [Header("Action Button")]
    public Button actionButton;
    public TextMeshProUGUI actionButtonText;

    private string currentSellerID;
    private ShopSlot selectedSlot;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // At start, the shop UI should be hidden
        gameObject.SetActive(false);
    }
    public void OpenShop(string sellerID)
    {
        currentSellerID = sellerID;
        gameObject.SetActive(true);
        infoPanel.SetActive(false); // At first, hide panel
        selectedSlot = null;
        Time.timeScale = 0f;
        
        RefreshGrids();
    }

    public void CloseShop()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private void RefreshGrids()
    {
        if (GameManager.Instance == null) return;
        SaveData save = GameManager.Instance.currentSaveData;

        // Sellers slots
        SellerData seller = save.sellerInventories.Find(s => s.sellerID == currentSellerID);
        for (int i = 0; i < sellerSlots.Length; i++)
        {
            if (seller != null && i < seller.availableItemIDs.Count)
            {
                ItemData item = GameManager.Instance.GetItemByID(seller.availableItemIDs[i]);
                sellerSlots[i].Setup(item, false); // isOwnedByPlayer = false
            }
            else
            {
                sellerSlots[i].Setup(null, false); // Boş slot
            }
        }

        // Player slots
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (i < save.inventoryItemIDs.Count)
            {
                ItemData item = GameManager.Instance.GetItemByID(save.inventoryItemIDs[i]);
                playerSlots[i].Setup(item, true); // isOwnedByPlayer = true
            }
            else
            {
                playerSlots[i].Setup(null, true); // Boş çanta slotu
            }
        }
    }

    // When a ShopSlot is selected, based on equipment type, show stats and enable buy/sell button
    public void SelectItem(ShopSlot slot)
    {
        selectedSlot = slot;
        infoPanel.SetActive(true);

        ItemData item = slot.currentItem;
        infoIcon.sprite = item.icon;
        infoName.text = $"{item.itemName} ({item.rarity})";
        
        string statsStr = item.description + "\n\n";
        if (item is EquipmentData eq)
        {
            if (eq.bonusHealth > 0) statsStr += $"Max HP: +{eq.bonusHealth}\n";
            if (eq.bonusDamage > 0) statsStr += $"Attack: +{eq.bonusDamage}\n";
            if (eq.bonusDefense > 0) statsStr += $"Defense: +{eq.bonusDefense}\n";
            if (eq.bonusCritChance > 0) statsStr += $"Crit: +{eq.bonusCritChance * 100f}%\n";
        }
        infoStats.text = statsStr;

        // Buy or sell logic
        if (slot.isOwnedByPlayer)
        {
            int sellPrice = Mathf.RoundToInt(item.coinValue * 0.5f); // 50% price for selling
            infoPrice.text = "Sell for: " + sellPrice + "G";
            
            actionButtonText.text = "SELL";
            actionButton.interactable = true; // You can always sell an item you own
        }
        else
        {
            infoPrice.text = "Buy for: " + item.coinValue + "G";
            
            actionButtonText.text = "BUY";
            bool canAfford = GameManager.Instance.currentSaveData.coins >= item.coinValue;
            actionButton.interactable = canAfford;
        }
    }

    // Executed when clicked on Action_Button 
    public void ExecuteTransaction()
    {
        if (selectedSlot == null || selectedSlot.currentItem == null) return;
        
        SaveData save = GameManager.Instance.currentSaveData;
        ItemData item = selectedSlot.currentItem;

        if (selectedSlot.isOwnedByPlayer) // Sell Process
        {
            int sellPrice = Mathf.RoundToInt(item.coinValue * 0.5f);
            save.coins += sellPrice;
            save.inventoryItemIDs.Remove(item.itemID);
            Debug.Log($"Sold {item.itemName} for {sellPrice}G");
        }
        else // Buy Process
        {
            if (save.coins >= item.coinValue)
            {
                save.coins -= item.coinValue;
                save.inventoryItemIDs.Add(item.itemID);
                
                SellerData seller = save.sellerInventories.Find(s => s.sellerID == currentSellerID);
                if (seller != null) seller.availableItemIDs.Remove(item.itemID);
                
                Debug.Log($"Bought {item.itemName} for {item.coinValue}G");
            }
        }

        // Refresh UI and coin display after transaction
        if (UIManager.Instance != null) UIManager.Instance.UpdateCoinDisplay();
        infoPanel.SetActive(false); 
        selectedSlot = null;
        RefreshGrids(); 
    }
}