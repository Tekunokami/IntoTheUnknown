using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    public Image healthFill; 

    [Header("Stats Panel UI")]
    public GameObject statsPanel;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI defenseText; 
    public TextMeshProUGUI critText;  
    public TextMeshProUGUI attackText;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthFill.fillAmount = currentHealth / maxHealth;
    }

    public void UpdateStatsDisplay()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerHealth healthComponent = player.GetComponent<PlayerHealth>();
        PlayerStats baseStats = healthComponent.baseStats;

        SaveData save = GameManager.Instance.currentSaveData;

        // Base Stats
        float totalMaxHealth = baseStats.maxHealth;
        float totalAttack = baseStats.attackDamage;
        float totalDefense = baseStats.defense;

        // Add up all bonuses from equipped items
        ItemData[] allGameItems = Resources.LoadAll<ItemData>("Items");
        foreach (string equipID in save.equippedItemIDs)
        {
            EquipmentData equipData = System.Array.Find(allGameItems, item => item.itemID == equipID) as EquipmentData;
            if (equipData != null)
            {
                totalMaxHealth += equipData.bonusHealth;
                totalAttack += equipData.bonusDamage;
            }
        }

        // Display the Totals
        healthText.text = $"Health: {healthComponent.currentHealth}/{totalMaxHealth}";
        defenseText.text = $"Defense: {totalDefense}"; 
        attackText.text = $"Attack: {totalAttack}";

        float critPercentage = baseStats.critRate * 100f;
        critText.text = $"Critical Chance: %{critPercentage:F1} (x{baseStats.critDamage})";
    }
}