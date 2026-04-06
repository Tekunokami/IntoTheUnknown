using UnityEngine;
using UnityEngine.UI; //Needed for Button 

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    void Start()
    {
        // Check if a save file exists and update the Continue button's interactability and appearance accordingly
        if (SaveManager.HasSave())
        {
            // If a save file exists, make the button interactable
            continueButton.interactable = true;
            continueButton.targetGraphic.color = Color.white;
        }
        else
        {
            // If no save file exists, make the button non-interactable
            continueButton.interactable = false;
            
            // For visual feedback (optional)
            Color fadedColor = continueButton.targetGraphic.color;
            fadedColor.a = 0.5f;
            continueButton.targetGraphic.color = fadedColor;
        }
    }

    // Button callback functions
    public void OnNewGameClicked()
    {
        GameManager.Instance.StartNewGame();
    }

    public void OnContinueClicked()
    {
        GameManager.Instance.ContinueGame();
    }

    public void OnQuitClicked()
    {
        Application.Quit();
        Debug.Log("Quit Game!"); 
    }
}