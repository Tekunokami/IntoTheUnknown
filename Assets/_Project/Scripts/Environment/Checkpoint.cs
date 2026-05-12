using UnityEngine;
using UnityEngine.InputSystem; 

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public string roomID;            // Ex: "room_1"
    public string spawnPointName;    // Ex: "Spawn_Checkpoint"
    
    [Header("UI")]
    public GameObject interactPrompt; // The floating "F" 
    private bool isPlayerInRange = false;

    private GameControls controls;

    private void Awake()
    {
        controls = new GameControls(); 
        controls.Player.Interact.performed += ctx => OnInteractPerformed();
    }

    private void Start()
    {
        // Prompt hidden at start
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void OnInteractPerformed()
    {
        // Triggered when player is in checkpoint range
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
            
            if (interactPrompt != null) interactPrompt.SetActive(true); // Show icon
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            controls.Disable(); // Stop listening to input
            
            if (interactPrompt != null) interactPrompt.SetActive(false); //Hide icon
        }
    }

    private void SaveGameAtCheckpoint()
    {
        if (GameManager.Instance != null)
        {
            // Update the respawn location to THIS checkpoint
            GameManager.Instance.currentSaveData.currentRoomID = roomID;
            GameManager.Instance.currentSaveData.currentSpawnPointName = spawnPointName;
            
            // Heal the player
            GameManager.Instance.currentSaveData.playerHealth = 100f; 

            // Save to active slot
            GameManager.Instance.SaveGame();

            Debug.Log($"[Checkpoint] Game Saved Successfully at {roomID}!");
        }
    }
}