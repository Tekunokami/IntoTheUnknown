using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Current Game State")]
    public SaveData currentSaveData;

    [Header("Temporary Session Data (Lost on Death)")]
    public int sessionCoins = 0;
    public List<string> sessionClearedEvents = new List<string>();

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
        if (sessionClearedEvents.Contains(eventID)) return true; 
        if (currentSaveData != null && currentSaveData.clearedEventIDs.Contains(eventID)) return true; 
        return false;
    }

    // Called when an enemy dies
    public void MarkEventCleared(string eventID, int coinValue)
    {
        if (!sessionClearedEvents.Contains(eventID))
        {
            sessionClearedEvents.Add(eventID);
            sessionCoins += coinValue;
        }
    }

    // Called by Checkpoints to save progress
    public void CommitSessionDataToSave()
    {
        if (currentSaveData == null) return;

        currentSaveData.coins += sessionCoins;
        
        foreach (string id in sessionClearedEvents)
        {
            if (!currentSaveData.clearedEventIDs.Contains(id))
            {
                currentSaveData.clearedEventIDs.Add(id);
            }
        }
        ClearSessionData(); 
    }

    // Called when the Player dies
    public void ClearSessionData()
    {
        sessionCoins = 0;
        sessionClearedEvents.Clear();
    }

    // For the UI to show the correct amount before saving
    public int GetCurrentDisplayCoins()
    {
        int bankedCoins = (currentSaveData != null) ? currentSaveData.coins : 0;
        return bankedCoins + sessionCoins;
    }
}