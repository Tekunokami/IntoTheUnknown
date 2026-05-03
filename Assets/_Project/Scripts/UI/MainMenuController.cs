using UnityEngine;
using UnityEngine.UI;//Needed for Button 

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject saveSlotPanel;

    [Header("Main Buttons")]
    [SerializeField] private Button continueButton;

    void Start()
    {
        // Check if ANY saveNumber has a save file to enable the main Continue button
        bool anySaveExists = false;

        for (int i = 1; i <= 6; i++)
        {
            if (SaveManager.HasSave(i))
            {
                anySaveExists = true;
                break; 
            }
        }

        continueButton.interactable = anySaveExists;
        
        if (!anySaveExists)
        {
            Color fadedColor = continueButton.targetGraphic.color;
            fadedColor.a = 0.5f;
            continueButton.targetGraphic.color = fadedColor;
        }

        // Ensure we start on the main panel
        ShowMainMenu();
    }


    // --- Navigation Functions ---
    public void ShowSaveSlots()
    {
        mainMenuPanel.SetActive(false);
        saveSlotPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        saveSlotPanel.SetActive(false);
    }


    // --- Button Callbacks ---
    public void OnNewGameClicked()
    {
        // Instead of starting, we show the slots so player can pick where to save
        ShowSaveSlots();
    }

    public void OnContinueClicked()
    {
        ShowSaveSlots();
    }

    public void OnBackClicked()
    {
        ShowMainMenu();
    }

    public void OnQuitClicked()
    {
        Application.Quit();
        Debug.Log("Quit Game!"); 
    }
}