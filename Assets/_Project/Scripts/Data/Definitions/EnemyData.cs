using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "GameData/Actors/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyID;
    public string enemyName;
    public GameObject prefab;
    public float baseHealth;

    [Header("Evolution System")]
    public List<AttackEvolution> attackEvolutions;
}

[System.Serializable]
public class AttackEvolution
{
    public int unlockFloor; // Attack unlocked at this floor
    public AttackData attack;
}