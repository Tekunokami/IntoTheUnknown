using UnityEngine;
using UnityEngine.InputSystem; 

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public string roomID;            // Ex: "room_2"
    public string spawnPointName;    // Ex: "Spawn_Checkpoint"
    
    private bool isPlayerInRange = false;

    private GameControls controls;

    private void Awake()
    {
        controls = new GameControls(); 
    }

    private void OnInteractPerformed()
    {
        // Only trigger if the player is in range of the checkpoint
        if (isPlayerInRange)
        {
            SaveGameAtCheckpoint();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            controls.Enable(); // Start listening to input
            
            Debug.Log("Press 'F' to Rest and Save.");
            // TODO: Show interaction icon (F) when player is in range
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            controls.Disable(); // Stop listening to input
            
            // TODO: Hide the interaction icon when player leaves
        }
    }
    private void SaveGameAtCheckpoint()
    {
        if (GameManager.Instance != null)
        {
            // 1. Update the respawn location to THIS checkpoint
            GameManager.Instance.currentSaveData.currentRoomID = roomID;
            GameManager.Instance.currentSaveData.currentSpawnPointName = spawnPointName;

            // 2. Fully heal the player
            GameManager.Instance.currentSaveData.playerHealth = 100f; 

            // 3. Save to the active slot
            GameManager.Instance.SaveGame();

            Debug.Log($"[Checkpoint] Game Saved Successfully at {roomID}!");
        }
    }
}