using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "GameData/Items/Consumable Data")]
public class ConsumableData : ItemData
{
    [Header("Consumable Effects")]
    public float healAmount;        
    public float tempDamageBuff; 

    [Header("Duration")]
    [Tooltip("How many combat rooms does this effect last?")]
    public int activeRoomDuration; 

    private void Awake()
    {
        itemType = ItemType.Consumable;
    }
}