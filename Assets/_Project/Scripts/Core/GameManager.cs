using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Current Game State")]
    public SaveData currentSaveData;


    void Awake()
    {   
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentSaveData = new SaveData();
        }
        else Destroy(gameObject);
    }

    public void StartNewGame(int saveNumber)
    {
        SaveManager.LoadFromNumber(saveNumber);
        currentSaveData = new SaveData(); 
        SaveGame(); 
        SceneManager.LoadScene("Test1Scene"); 
    }

    public void ContinueGame(int saveNumber)
    {
        if (SaveManager.HasSave(saveNumber))
        {
            currentSaveData = SaveManager.LoadFromNumber(saveNumber);
            SceneManager.LoadScene("Test1Scene");
        }
        else Debug.LogWarning($"Could not find save file in Save {saveNumber}!");
    }

    public void SaveGame()
    {
        SaveManager.Save(currentSaveData);
    }

    // ---Enemy and Event Tracking Methods---

    // Checks if an enemy/event is dead in the temporary session OR the saved file
    public bool IsEventCleared(string eventID)
    {
        return currentSaveData != null && currentSaveData.clearedEventIDs.Contains(eventID);
    }

    
    public void ReloadFromLastSave()
    {
        // Overwrite active memory with the hard drive's backup
        currentSaveData = SaveManager.Load(); 
        Debug.Log("<color=red>Rolled back to last save state!</color>");
    }


    // For the UI to show the correct amount before saving
    public int GetCurrentDisplayCoins()
    {
        return (currentSaveData != null) ? currentSaveData.coins : 0;
    }

}