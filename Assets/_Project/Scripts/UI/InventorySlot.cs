using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType { Bag, Head, Body, Weapon, Accessory }
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public SlotType slotType;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        DraggableItem draggableItem = droppedObject.GetComponent<DraggableItem>();

        if (draggableItem != null)
        {
            EquipmentData equipData = draggableItem.itemData as EquipmentData;
            InventorySlot startingSlot = draggableItem.originalParent.GetComponent<InventorySlot>();

            // Do nothing if same spot
            if (startingSlot == this) return;

            bool isValidDrop = false;

            // Bag accepts anything
            if (slotType == SlotType.Bag) isValidDrop = true;
            // Equip slots only accept matching types
            else if (equipData != null && equipData.equipSlot.ToString() == slotType.ToString()) isValidDrop = true;

            // Check if the slot is truly empty
            DraggableItem itemInThisSlot = GetComponentInChildren<DraggableItem>();
            bool hasRealItem = (itemInThisSlot != null && itemInThisSlot.itemData != null);
            
            if (hasRealItem) isValidDrop = false;

            if (isValidDrop)
            {
                // Destroy the invisible ghost box 
                if (itemInThisSlot != null && itemInThisSlot.itemData == null)
                {
                    Destroy(itemInThisSlot.gameObject);
                }

                // Snap visually
                draggableItem.originalParent = transform;
                draggableItem.transform.SetParent(transform);
                draggableItem.transform.localPosition = Vector3.zero;

                // Update the Backend Save Data
                UpdateSaveData(draggableItem.itemData, startingSlot.slotType, this.slotType);
            }
        }
    }

    private void UpdateSaveData(ItemData item, SlotType fromSlot, SlotType toSlot)
    {
        // If we are just moving the item to a different slot inside the bag, dont change save data
        if (fromSlot == toSlot) return; 

        SaveData save = GameManager.Instance.currentSaveData;

        // Remove from old list
        if (fromSlot == SlotType.Bag) save.inventoryItemIDs.Remove(item.itemID);
        else save.equippedItemIDs.Remove(item.itemID);

        // Add to new list
        if (toSlot == SlotType.Bag) save.inventoryItemIDs.Add(item.itemID);
        else save.equippedItemIDs.Add(item.itemID);

        // Recalculate Stats!
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateStatsDisplay();
        }
    }
}