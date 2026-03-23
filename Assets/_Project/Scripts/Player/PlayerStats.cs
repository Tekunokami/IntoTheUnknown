using UnityEngine;
[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Stats/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Base Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float defense = 5f;
    public float attackDamage = 10f;
    public float attackSpeed = 1f;

    [Header("Crit Stats")]
    public float critRate = 0.1f; 
    public float critDamage = 2f; 

    public void ResetStats()
    {
        currentHealth = maxHealth;
    }
}
