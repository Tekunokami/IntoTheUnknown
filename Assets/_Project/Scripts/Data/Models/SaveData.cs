using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // Current game state info to save
    public int currentFloor;
    public string currentRoomID; 
    public string lastRoomID;
    public string currentSpawnPointName;

    // player stats and inventory
    public float playerHealth;
    public int coins;
    public List<string> inventoryItemIDs = new List<string>(); // A list of itemIDs that are in player's bag
    public List<EquipSlot> equippedSlots = new List<EquipSlot>();
    public List<string> equippedItemIDs = new List<string>();
    public List<string> activeConsumableIDs = new List<string>();
    public List<int> activeConsumableDurations = new List<int>();

    // World state info to save (which rooms/events have been cleared)
    public List<string> clearedEventIDs; 

    public SaveData()
    {
        currentFloor = 1;
        currentRoomID = "room_1"; 
        lastRoomID = "";
        currentSpawnPointName = "Spawn_Left";
        
        playerHealth = 100f;
        coins = 0;
        
        inventoryItemIDs = new List<string>();
        equippedSlots = new List<EquipSlot>();
        equippedItemIDs = new List<string>();
        activeConsumableIDs = new List<string>();
        activeConsumableDurations = new List<int>();
        clearedEventIDs = new List<string>(); // Initially no cleared events
    }
}