using UnityEngine;

// An enum to help the UI with type specific logic
public enum ItemType { Equipment, Consumable, Material }

public class ItemData : ScriptableObject
{
    [Header("Base Item Info")]
    public string itemID;        // Ex: "iron_sword_01"
    public string itemName;      // Ex: "Iron Sword"
    [TextArea]
    public string description;  // Text shown when item is selected
    
    public Sprite icon;          // The picture of item
    public ItemType itemType;
    public int coinValue;        // Value of item
}