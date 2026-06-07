using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Image icon;
    
    [HideInInspector] public ItemData currentItem;
    [HideInInspector] public bool isOwnedByPlayer; // True for selling, False for buying

    public void Setup(ItemData item, bool playerOwned)
    {
        currentItem = item;
        isOwnedByPlayer = playerOwned;

        if (item != null)
        {
            icon.sprite = item.icon;
            icon.color = Color.white;
            GetComponent<Button>().interactable = true;
        }
        else
        {
            icon.sprite = null;
            icon.color = Color.clear;
            GetComponent<Button>().interactable = false; // Prevent clicking empty slots
        }
    }

    public void OnSlotClicked()
    {
        if (currentItem != null && ShopUI.Instance != null)
        {
            // Tell the main ShopUI manager that this slot was clicked
            ShopUI.Instance.SelectItem(this);
        }
    }
}