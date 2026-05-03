using UnityEngine;

public enum EquipSlot { Head, Body, Weapon, Accessory }

[CreateAssetMenu(fileName = "NewEquipment", menuName = "GameData/Items/Equipment Data")]
public class EquipmentData : ItemData
{
    [Header("Equipment Settings")]
    public EquipSlot equipSlot;

    [Header("Stat Bonuses")]
    public float bonusHealth;
    public float bonusDamage;
    //TODO:Add more stats here later like bonusSpeed, critChance

    private void Awake()
    {
        itemType = ItemType.Equipment;
    }
}