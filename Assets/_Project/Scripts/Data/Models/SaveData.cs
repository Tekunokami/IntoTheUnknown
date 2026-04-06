using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // World Progress
    public int currentFloor;
    public int currentRoomIndex;
    public string lastRoomID;

    // Player Progress
    public float playerHealth;
    public int coins;
    public List<string> inventoryItemIDs = new List<string>();

    // Initialize with default values (New Game)
    public SaveData()
    {
        currentFloor = 1;
        currentRoomIndex = 0;
        playerHealth = 100f;
        coins = 0;
    }
}