using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Current Game State")]
    public SaveData currentSaveData;

    // Temporary in-memory database for item information
    private Dictionary<string, ItemData> itemDatabase = new Dictionary<string, ItemData>();

    void Awake()
    {   
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentSaveData = new SaveData();

            LoadItemDatabase(); //Load item data from database
        }
        else Destroy(gameObject);
    }


    // ---Item Database Methods---
    private void LoadItemDatabase()
    {
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
        foreach (ItemData item in allItems)
        {
            if (!itemDatabase.ContainsKey(item.itemID))
            {
                itemDatabase.Add(item.itemID, item);
            }
        }
        Debug.Log($"[Database] Successfully loaded {itemDatabase.Count} items into memory.");
    }
    
    // Retrieves item data by ID
    public ItemData GetItemByID(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        
        if (itemDatabase.TryGetValue(id, out ItemData item))
        {
            return item;
        }
        
        Debug.LogWarning($"[Database] '{id}' item not found!");
        return null;
    }


    // Lists all items in the database
    public List<ItemData> GetAllItems()
    {
        return new List<ItemData>(itemDatabase.Values);
    }


    void Update()
    {
        if (currentSaveData != null)
        {
            currentSaveData.totalPlayTime += Time.deltaTime;
        }
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