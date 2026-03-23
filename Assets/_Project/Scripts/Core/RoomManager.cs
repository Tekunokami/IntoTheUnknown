using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("Room Settings")]
    public List<GameObject> roomPrefabs; 
    public Transform spawnPoint; 

    private GameObject currentRoom;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnNewRoom();
    }

    public void SpawnNewRoom()
    {
        if (currentRoom != null) {
            Destroy(currentRoom);
        }

        int randomIndex = Random.Range(0, roomPrefabs.Count);
        
        currentRoom = Instantiate(roomPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            player.transform.position = spawnPoint.position; 
        }
    }
}