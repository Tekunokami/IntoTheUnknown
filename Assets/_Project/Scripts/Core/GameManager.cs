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

    // Start a new game (Reset everything)
    public void StartNewGame()
    {
        currentSaveData = new SaveData(); // Initialize with default values
        SceneManager.LoadScene("Test1Scene"); 
    }

    // Read from disk and load the game state
    public void ContinueGame()
    {
        if (SaveManager.HasSave())
        {
            currentSaveData = SaveManager.Load();
            SceneManager.LoadScene("Test1Scene");
        }
        else
        {
            Debug.LogWarning("Could not find save file!");
        }
    }

    // Save Game to the disk
    public void SaveGame()
    {
        SaveManager.Save(currentSaveData);
    }
}