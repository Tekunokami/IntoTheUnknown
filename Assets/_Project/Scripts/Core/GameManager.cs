using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Current Game State")]
    public SaveData currentSaveData;

    void Awake()
    {
        // There should be only one GameManager in the scene at any time. If another exists, destroy this one.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Dont destroy this object when loading new scenes
            currentSaveData = new SaveData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

   // Start a new game on a specific save number (User selects a save from the UI)
    public void StartNewGame(int saveNumber)
    {
        // Set the active save number in SaveManager
        SaveManager.LoadFromNumber(saveNumber);
        
        // Initialize with default values
        currentSaveData = new SaveData(); 
        
        // Save immediately so the slot registers as "taken" on the hard drive
        SaveGame(); 
        
        SceneManager.LoadScene("Test1Scene"); 
    }

    // Read from disk and load the game state
    public void ContinueGame(int saveNumber)
    {
        if (SaveManager.HasSave(saveNumber))
        {
            currentSaveData = SaveManager.LoadFromNumber(saveNumber);
            SceneManager.LoadScene("Test1Scene");
        }
        else
        {
            Debug.LogWarning($"Could not find save file in Save {saveNumber}!");
        }
    }

    // Save Game to the disk
    public void SaveGame()
    {
        SaveManager.Save(currentSaveData);
    }
}