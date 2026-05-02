using UnityEngine;
using UnityEngine.UI;
using TMPro; // Used for better Text

public class SaveSlotUI : MonoBehaviour
{
    [Header("Slot Settings")]
    [Range(1, 6)] public int slotNumber = 1;

    [Header("UI Elements")]
    public TextMeshProUGUI slotNumberText; 
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
        hasData = SaveManager.HasSave(slotNumber);
        slotNumberText.text = $"SLOT {slotNumber}";

        if (hasData)
        {
            // Load the data to display
            SaveData data = SaveManager.LoadFromSlot(slotNumber);
            locationText.text = data.currentRoomID; //Later translete roomID to understandvable location names
            statsText.text = $"Health: {data.playerHealth}   Coins: {data.coins}";
            
            deleteButton.gameObject.SetActive(true);
        }
        else
        {
            locationText.text = "Empty Slot";
            statsText.text = "Start a new adventure";

            deleteButton.gameObject.SetActive(false);
        }
    }

    // Called when the main slot button is clicked
    public void OnSlotClicked()
    {
        if (hasData)
        {
            GameManager.Instance.ContinueGame(slotNumber);
        }
        else
        {
            GameManager.Instance.StartNewGame(slotNumber);
        }
    }

    public void OnDeleteClicked()
    {
        SaveManager.DeleteSave(slotNumber);
        UpdateSlotDisplay();
    }
}