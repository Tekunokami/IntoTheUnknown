using UnityEngine;

// An enum to help the UI with type specific logic
public enum ItemType { Equipment, Consumable, Material }
public enum ItemRarity { Poor, Common, Rare, Epic }
public class ItemData : ScriptableObject
{
    [Header("Base Item Info")]
    public string itemID;        // Ex: "iron_sword_01"
    public string itemName;      // Ex: "Iron Sword"
    [TextArea]
    public string description; 
    
    public Sprite icon;        
    public ItemType itemType;
    public ItemRarity rarity;
    public int coinValue;       
}