using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Button buyButton;

    private ItemData currentItem;
    private string currentSellerID;

    public void SetupSlot(ItemData item, string sellerID)
    {
        currentItem = item;
        currentSellerID = sellerID;

        if (item != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.color = Color.white;
            itemNameText.text = item.itemName;
            itemPriceText.text = item.coinValue.ToString() + "G";

            // Eğer oyuncunun parası yetmiyorsa butonu tıklanamaz (gri) yap
            bool canAfford = GameManager.Instance.currentSaveData.coins >= item.coinValue;
            buyButton.interactable = canAfford;
        }
        else
        {
            // Eğer eşya satılmışsa (null gelirse) slotu boş/tükendi göster
            itemIcon.sprite = null;
            itemIcon.color = Color.clear;
            itemNameText.text = "Out of Stock";
            itemPriceText.text = "-";
            buyButton.interactable = false;
        }
    }

    // Bu metodu Unity'de BuyButton'un OnClick eventine bağlayacağız
    public void OnBuyClicked()
    {
        if (currentItem == null || GameManager.Instance == null) return;

        SaveData save = GameManager.Instance.currentSaveData;

        if (save.coins >= currentItem.coinValue)
        {
            // 1. Parayı kes
            save.coins -= currentItem.coinValue;

            // 2. Eşyayı oyuncunun çantasına ekle
            save.inventoryItemIDs.Add(currentItem.itemID);

            // 3. Eşyayı satıcının envanterinden sil (Tükendi yapmak için)
            SellerData seller = save.sellerInventories.Find(s => s.sellerID == currentSellerID);
            if (seller != null)
            {
                seller.availableItemIDs.Remove(currentItem.itemID);
            }

            Debug.Log($"<color=green>Satın alındı: {currentItem.itemName}</color>");

            // 4. UI'ı anında güncelle (Parayı ve dükkan raflarını yenile)
            UIManager.Instance.UpdateCoinDisplay();
            UIManager.Instance.RefreshShopUI(currentSellerID); 
        }
    }
}