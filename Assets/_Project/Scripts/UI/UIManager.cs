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

        PlayerStats stats = player.GetComponent<PlayerHealth>().baseStats;

        healthText.text = $"Health: {stats.currentHealth}/{stats.maxHealth}";
        
        defenseText.text = $"Defense: {stats.defense}"; 
        
        attackText.text = $"Attack: {stats.attackDamage}";

        float critPercentage = stats.critRate * 100f;
        critText.text = $"Critical Chance: %{critPercentage:F1} (x{stats.critDamage})";
    }
}