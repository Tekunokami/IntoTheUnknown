using UnityEngine;
using UnityEngine.InputSystem;

public enum RoomType { Corridor, Combat, Shop, Boss }
public class DoorTransition : MonoBehaviour
{
    [Header("Destination Settings")]
    public RoomData destinationRoom;
    public string targetSpawnPointName; 
    
    [Header("Room Type Settings")]
    public RoomType targetRoomType;
    public int requiredCoinsForShop = 50;

    private bool isPlayerInRange = false;
    private GameControls controls;
    private void Awake()
    {
        controls = new GameControls();
        
        // When the player presses the interaction key, we will call the function to change rooms.
        controls.Player.Interact.performed += ctx => OnInteractPerformed();
    }
   
    private void OnInteractPerformed()
    {
        // Only trigger if the player is in range of the door and presses the interaction key.
        if (isPlayerInRange && destinationRoom != null)
        {
            // Room type checks before allowing transition
            if (destinationRoom.roomType == RoomType.Shop && GameManager.Instance.currentSaveData.coins < requiredCoinsForShop)
            {
                Debug.Log($"Door is locked! You need at least {requiredCoinsForShop} coins to enter.");
                // TODO: Not enough coins message
                return; 
            }

            // If it's a combat or boss room, when entered its considered as cleared
            if (destinationRoom.roomType == RoomType.Combat || destinationRoom.roomType == RoomType.Boss)
            {
                if (!GameManager.Instance.currentSaveData.clearedEventIDs.Contains(destinationRoom.roomID))
            {
                    // New room, increase difficulty
                    GameManager.Instance.currentSaveData.roomsClearedCount++;
                   
                   // Mark this as cleared
                    GameManager.Instance.currentSaveData.clearedEventIDs.Add(destinationRoom.roomID);
                    
                    Debug.Log($"First time entering {destinationRoom.roomID}. Difficulty increased!");
                }
            }

            RoomManager.Instance.ChangeRoom(destinationRoom.roomID, targetSpawnPointName);
            
            Debug.Log($"[Door] Changed to {destinationRoom.roomID}! Type: {destinationRoom.roomType}");
            
            isPlayerInRange = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            controls.Enable(); // When player enters the door area, start listening to the interaction key
            
            // TODO: Show interaction icon (E) when player is in range
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            controls.Disable(); // When player leaves the door area, stop listening to the interaction key
            
            // TODO: Hide the interaction icon when player leaves
        }
    }
}