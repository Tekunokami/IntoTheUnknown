using UnityEngine;
using UnityEngine.UI;//Needed for Button 

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject saveSlotPanel;
    [SerializeField] private GameObject savesFullPanel;

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

        if (savesFullPanel != null) savesFullPanel.SetActive(false); // Ensure Saves Full panel is hidden at  start
        
        ShowMainMenu();  // Ensure we start on the main panel
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


    // --- Button Functions ---
    public void OnNewGameClicked()
    {
        // Look if any empty save slot exists
        for (int i = 1; i <= 6; i++)
        {
            if (!SaveManager.HasSave(i))
            {
                // Empty save found, start new game
                GameManager.Instance.StartNewGame(i);
                return; 
            }
        }

        if (savesFullPanel != null)
        {
            savesFullPanel.SetActive(true);
        }
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

    public void CloseSavesFullWarning()
    {
        savesFullPanel.SetActive(false);
    }
}