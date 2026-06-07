using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    public Image healthFill; 
    public TMPro.TextMeshProUGUI coinText; 
    public GameObject deathScreenPanel; 
    public TMPro.TextMeshProUGUI deathScreenText;

    [Header("Stats Panel UI")]
    public GameObject statsPanel;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI defenseText; 
    public TextMeshProUGUI critText;  
    public TextMeshProUGUI attackText;

    [Header("Death Screen UI References")]
    public GameObject deathPanel;
    public TMPro.TextMeshProUGUI deathMessageText;
    public TMPro.TextMeshProUGUI statsText;

    [Header("Victory Screen UI")]
    public GameObject victoryPanel;
    public TMPro.TextMeshProUGUI victoryStatsText; 
    public TMPro.TextMeshProUGUI victoryEquipText;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthFill.fillAmount = currentHealth / maxHealth;
    }

    void Start()
    {
        // Initialize coin display 
        UpdateCoinDisplay();
    }

    public void UpdateStatsDisplay()
    {
        // Access player stats from PlayerHealth singleton
        if (PlayerHealth.Instance == null) return;

        PlayerStats baseStats = PlayerHealth.Instance.baseStats;

        float totalMaxHealth = PlayerHealth.Instance.GetTotalMaxHealth();
        float totalAttack = PlayerHealth.Instance.GetTotalAttackDamage();
        float totalDefense = PlayerHealth.Instance.GetTotalDefense(); // Uzmandan çekildi
        float totalCritChance = PlayerHealth.Instance.GetTotalCritChance(); // Uzmandan çekildi


        // Add bonuses from equipped items
        healthText.text = $"Health: {PlayerHealth.Instance.currentHealth}/{totalMaxHealth}";
        defenseText.text = $"Defense: {totalDefense}"; 
        attackText.text = $"Attack: {totalAttack}";

        float critPercentage = totalCritChance * 100f;
        critText.text = $"Critical Chance: %{critPercentage:F1} (x{baseStats.critDamage})";
    }
    public void UpdateCoinDisplay()
    {
        if (coinText != null && GameManager.Instance != null)
        {
            coinText.text = "Coins: " + GameManager.Instance.GetCurrentDisplayCoins();
        }
    }

    public void ShowDeathScreen(string message)
    {
        if (deathPanel != null) deathPanel.SetActive(true);
        if (deathMessageText != null) deathMessageText.text = message;

        if (GameManager.Instance != null && statsText != null)
        {
            SaveData save = GameManager.Instance.currentSaveData;

           // Format total play time into hrs:min:sec
            System.TimeSpan timePlaying = System.TimeSpan.FromSeconds(save.totalPlayTime);
            string formattedTime = timePlaying.ToString(@"hh\:mm\:ss");

            statsText.text = $"Total Play Time: {formattedTime}\n" +
                             $"Rooms Cleared: {save.roomsClearedCount}\n" +
                             $"Enemies Killed: {save.enemiesKilledCount}";
        }
    }

    public void ShowVictoryScreen()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);

        if (GameManager.Instance != null)
        {
            SaveData save = GameManager.Instance.currentSaveData;

            System.TimeSpan timePlaying = System.TimeSpan.FromSeconds(save.totalPlayTime);
            string formattedTime = timePlaying.ToString(@"hh\:mm\:ss");

            // Calculate final stats including equipment bonuses for display on victory screen
            float finalMaxHP = 0, finalAttack = 0, finalDefense = 0, finalCrit = 0;

            if (PlayerHealth.Instance != null)
            {
                finalMaxHP = PlayerHealth.Instance.GetTotalMaxHealth();
                finalAttack = PlayerHealth.Instance.GetTotalAttackDamage();
                finalDefense = PlayerHealth.Instance.GetTotalDefense();
                finalCrit = PlayerHealth.Instance.GetTotalCritChance() * 100f;
            }

            if (victoryStatsText != null)
            {
                victoryStatsText.text = 
                    $"Total Time Spent: <color=#FFD700>{formattedTime}</color>\n" +
                    $"Enemies Killed: <color=#FF4500>{save.enemiesKilledCount}</color>\n" +
                    $"Total Coins Looted: <color=#FFFF00>{save.totalCoinsLooted}</color>\n" +
                    $"Total Damage Taken: <color=#FF0000>{save.totalDamageTaken}</color>\n\n" +
                    $"-- FINAL STATS --\n" +
                    $"Max HP: {finalMaxHP} | Attack: {finalAttack} | Defense: {finalDefense} | Crit: %{finalCrit:F1}";
            }

            if (victoryEquipText != null)
            {
                string equipString = "EQUIPPED ITEMS:\n";
                if (save.equippedItemIDs.Count == 0)
                {
                    equipString += "<color=grey>No Equipment Used (Challenge Run!)</color>";
                }
                else
                {
                    // List equipped items by name and type
                    foreach (string itemID in save.equippedItemIDs)
                    {
                        ItemData item = GameManager.Instance.GetItemByID(itemID);
                        if (item != null)
                        {
                            equipString += $"- {item.itemName} ({item.itemType})\n";
                        }
                    }
                }
                victoryEquipText.text = equipString;
            }

            Time.timeScale = 0f; 
        }
    }



    

    

  
}