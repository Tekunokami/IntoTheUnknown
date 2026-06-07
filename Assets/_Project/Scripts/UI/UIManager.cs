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

            // Time format
            System.TimeSpan timePlaying = System.TimeSpan.FromSeconds(save.totalPlayTime);
            string formattedTime = timePlaying.ToString(@"hh\:mm\:ss");

            // Stats
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            float finalMaxHP = 0, finalAttack = 0, finalDefense = 0;

            if (player != null && player.TryGetComponent(out PlayerHealth ph))
            {
                finalMaxHP = ph.GetTotalMaxHealth();
                finalAttack = ph.baseStats.attackDamage;
                finalDefense = ph.baseStats.defense;
                
                // Equip bonuses
                ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
                foreach (string equipID in save.equippedItemIDs)
                {
                    EquipmentData eq = System.Array.Find(allItems, i => i.itemID == equipID) as EquipmentData;
                    if (eq != null)
                    {
                        finalAttack += eq.bonusDamage;
                    }
                }
            }

            // Final Stats Summary
            if (victoryStatsText != null)
            {
                victoryStatsText.text = 
                    $"TOTAL TIME: <color=#FFD700>{formattedTime}</color>\n" +
                    $"ENEMIES KILLED: <color=#FF4500>{save.enemiesKilledCount}</color>\n" +
                    $"TOTAL COINS LOOTED: <color=#FFFF00>{save.totalCoinsLooted}</color>\n" +
                    $"TOTAL DAMAGE TAKEN: <color=#FF0000>{save.totalDamageTaken}</color>\n\n" +
                    $"-- FINAL STATS --\n" +
                    $"Max HP: {finalMaxHP} | Attack: {finalAttack} | Defense: {finalDefense}";
            }

            // Final Equipment List
            if (victoryEquipText != null)
            {
                string equipString = "EQUIPPED ITEMS:\n";
                if (save.equippedItemIDs.Count == 0)
                {
                    equipString += "<color=grey>No Equipment Used (Challenge Run!)</color>";
                }
                else
                {
                    ItemData[] allGameItems = Resources.LoadAll<ItemData>("Items");
                    foreach (string itemID in save.equippedItemIDs)
                    {
                        ItemData item = System.Array.Find(allGameItems, i => i.itemID == itemID);
                        if (item != null)
                        {
                            equipString += $"- {item.itemName} ({item.itemType})\n";
                        }
                    }
                }
                victoryEquipText.text = equipString;
            }

            //  Prevent player from moving
            Time.timeScale = 0f; 
        }
    }
}