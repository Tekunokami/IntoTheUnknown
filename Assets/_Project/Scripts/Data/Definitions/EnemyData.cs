using UnityEngine;
public enum EnemyRarity { Common, Elite, Boss }

[CreateAssetMenu(fileName = "NewEnemy", menuName = "GameData/Actors/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyID;
    public string enemyName;

    [Header("Core Stats")]
    public float maxHealth = 30f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Combat")]
    public float attackDamage = 10f;
    public float detectionRange = 6f;
    public float attackRange = 1.5f; 
    public float attackCooldown = 1.5f;

    [Header("Rewards & Identity")]
    public EnemyRarity rarity; 
    public int coinValue = 10;
}