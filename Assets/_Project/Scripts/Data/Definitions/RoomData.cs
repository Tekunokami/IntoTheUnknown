using UnityEngine;

[CreateAssetMenu(fileName = "NewRoom", menuName = "GameData/World/Room Data")]
public class RoomData : ScriptableObject
{
    [Header("Room Info")]
    public string roomID;
    public GameObject roomPrefab;

}