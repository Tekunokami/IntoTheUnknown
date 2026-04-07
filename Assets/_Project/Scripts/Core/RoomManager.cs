using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("Room Database")]
    // We will assign all RoomData (SO) files here in Inspector
    public List<RoomData> allRooms; 
    
    private GameObject currentRoomObj;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Prevent multiple instances
    }

    void Start()
    {
        // If GameManager exists, load the room based on the saved ID 
        if (GameManager.Instance != null)
        {
            LoadRoomByID(
                GameManager.Instance.currentSaveData.currentRoomID, 
                GameManager.Instance.currentSaveData.currentSpawnPointName
            );
        }
    }

    // Function triggered by the doors (DoorTransition script)
    public void ChangeRoom(string targetRoomID, string targetSpawnPointName)
    {
        // Update the save data with the new room ID 
        GameManager.Instance.currentSaveData.currentRoomID = targetRoomID;

        // Save the spawn point name for the new room 
        GameManager.Instance.currentSaveData.currentSpawnPointName = targetSpawnPointName;
        
        // Auto-save the game
        GameManager.Instance.SaveGame();

        // Load the new room
        LoadRoomByID(targetRoomID, targetSpawnPointName);
    }

    private void LoadRoomByID(string roomID, string spawnPointName)
    {
        // Find the room with the matching ID in our list
        RoomData roomToLoad = allRooms.Find(r => r.roomID == roomID);

        if (roomToLoad != null)
        {
            SpawnRoom(roomToLoad.roomPrefab, spawnPointName);
        }
        else
        {
            Debug.LogError($"ERROR: Room with ID '{roomID}' not found!");
        }
    }

    private void SpawnRoom(GameObject roomPrefab, string spawnPointName)
    {
        // Clear the old room from the scene
        if (currentRoomObj != null) 
        {
            Destroy(currentRoomObj);
        }

        // Instantiate the new room at the center (0,0,0)
        currentRoomObj = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
        
        // --- TELEPORT PLAYER TO THE DOOR ---
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) 
        {
            // If a specific spawn point was requested by the door 
            if (!string.IsNullOrEmpty(spawnPointName))
            {
                Transform targetSpawn = currentRoomObj.transform.Find(spawnPointName);
                if (targetSpawn != null)
                {
                    player.transform.position = targetSpawn.position;
                }
                else
                {
                    Debug.LogWarning($"Warning: Spawn point '{spawnPointName}' not found in the room! Teleporting player to the center.");
                    player.transform.position = Vector3.zero;
                }
            }
            else
            {
                // If no target is specified, put them in the center
                player.transform.position = Vector3.zero;
            }
        }
    }
}