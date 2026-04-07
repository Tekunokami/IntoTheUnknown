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
    public List<string> inventoryItemIDs;

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
        clearedEventIDs = new List<string>(); // Initially no cleared events
    }
}