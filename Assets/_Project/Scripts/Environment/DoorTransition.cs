using UnityEngine;
using UnityEngine.InputSystem;
public class DoorTransition : MonoBehaviour
{
    [Header("Destination Settings")]
    public string targetRoomID; 
    public string targetSpawnPointName; 

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
        if (isPlayerInRange)
        {
            RoomManager.Instance.ChangeRoom(targetRoomID, targetSpawnPointName);
            
            Debug.Log($"[Door] {targetRoomID} Room Changed!");
            
            isPlayerInRange = false; // To prevent multiple triggers (spamming)
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