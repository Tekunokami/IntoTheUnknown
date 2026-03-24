using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private GameControls _controls;

    [Header("UI Elements")]
    public Image healthFill; 

    [Header("Stats Panel UI")]
    public GameObject statsPanel;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI dashText;
    public TextMeshProUGUI attackText;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        _controls = new GameControls();
    }

    private void OnEnable()
    {
        _controls.Player.Enable();
        _controls.Player.ToggleStats.performed += ctx => ToggleStatsPanel();
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
    }

    
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthFill.fillAmount = currentHealth / maxHealth;
    }

    public void ToggleStatsPanel()
    {
        bool isActive = !statsPanel.activeSelf;
        statsPanel.SetActive(isActive);

        if (isActive) 
        {
            UpdateStatsDisplay();
        }
    }

    void UpdateStatsDisplay()
    {
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player == null) return;

    PlayerStats stats = player.GetComponent<PlayerHealth>().baseStats;

    healthText.text = "Health: " + stats.maxHealth;
    speedText.text = "Defense: " + stats.defense; 
    attackText.text = "Attack: " + stats.attackDamage + " (Speed: " + stats.attackSpeed + ")";

    float critYuzde = stats.critRate * 100f;
    dashText.text = "Critical Chance: %" + critYuzde + " (x" + stats.critDamage + ")";
}
}