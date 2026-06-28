using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI; 

    private GameControls controls;
    private bool isPaused = false;

    void Awake()
    {
        controls = new GameControls();
        
        // Trigger the TogglePause method
        controls.Player.Pause.performed += ctx => TogglePause();
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    public void TogglePause()
    {
        // Eğer envanter falan açıksa pause menüsünün çakışmasını engellemek isteyebilirsin
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    // --- Button Methods ---

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Continue the game 
        isPaused = false;
    }

    private void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Stop the game 
        isPaused = true;
    }

    public void LoadMainMenuButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void QuitGameButton()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}