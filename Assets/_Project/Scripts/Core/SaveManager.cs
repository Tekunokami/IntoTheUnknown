using System.IO;
using UnityEngine;

public static class SaveManager
{
    // Keeps track of the active save number for auto-saves during room transitions.
    // Defaults to save 1.
    public static int CurrentSaveNumber { get; private set; } = 1;

    // Generates a unique file path based on the save number
    private static string GetPath(int saveNumber)
    {
        return Path.Combine(Application.persistentDataPath, $"save_file_{saveNumber}.json");
    }

    // Checks if a save file exists for the given save number
    public static bool HasSave(int saveNumber)
    {
        return File.Exists(GetPath(saveNumber));
    }
    // --- SAVE OPERATIONS ---
    public static void Save(SaveData data)
    {
        SaveToNumber(data, CurrentSaveNumber);
    }

    // 2. For saving at checkpoints
    public static void SaveToNumber(SaveData data, int saveNumber)
    {
        CurrentSaveNumber = saveNumber; 
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(saveNumber), json);
    }

    // --- LOAD OPERATIONS ---

    // For in-game quick loads (like respawning after death in the current save)
    public static SaveData Load()
    {
        return LoadFromNumber(CurrentSaveNumber);
    }

    // 2. For loading a specific save from the UI
    public static SaveData LoadFromNumber(int saveNumber)
    {
        CurrentSaveNumber = saveNumber; // Set the chosen save number as the active one
        string path = GetPath(saveNumber);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        
        Debug.LogWarning($"Save {saveNumber} is empty. Creating new save data.");
        return new SaveData(); 
    }

    // For DeleteButton in the UI
    public static void DeleteSave(int saveNumber)
    {
        string path = GetPath(saveNumber);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Save {saveNumber} successfully deleted.");
        }
    }
}