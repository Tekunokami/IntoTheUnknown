using UnityEngine;
using UnityEngine.UI;
using TMPro; // Used for better Text

public class SaveSlotUI : MonoBehaviour
{
    [Header("Save Settings")]
    [Range(1, 6)] public int saveNumber = 1;

    [Header("UI Elements")]
    public TextMeshProUGUI saveNumberText; 
    public TextMeshProUGUI locationText;   
    public TextMeshProUGUI statsText;   
    public Button deleteButton;      

    private bool hasData = false;

    private void OnEnable()
    {
        UpdateSlotDisplay();
    }

    public void UpdateSlotDisplay()
    {
        hasData = SaveManager.HasSave(saveNumber);
        saveNumberText.text = $"SAVE {saveNumber}";

        if (hasData)
        {
            // Load the data to display
            SaveData data = SaveManager.LoadFromNumber(saveNumber);
            locationText.text = data.currentRoomID; //Later translete roomID to understandvable location names
            statsText.text = $"Health: {data.playerHealth}   Coins: {data.coins}";

            deleteButton.gameObject.SetActive(true);
        }
        else
        {
            locationText.text = "Empty Save";
            statsText.text = "Start a new adventure";

            deleteButton.gameObject.SetActive(false);
        }
    }

    // Called when the main slot button is clicked
    public void OnSlotClicked()
    {
        if (hasData)
        {
            GameManager.Instance.ContinueGame(saveNumber);
        }
        else
        {
            GameManager.Instance.StartNewGame(saveNumber);
        }
    }

    public void OnDeleteClicked()
    {
        SaveManager.DeleteSave(saveNumber);
        UpdateSlotDisplay();
    }
}