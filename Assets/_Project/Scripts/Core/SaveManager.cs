using System.IO;
using UnityEngine;

public static class SaveManager
{
    // Keeps track of the active slot for auto-saves during room transitions.
    // Defaults to slot 1.
    public static int CurrentSlot { get; private set; } = 1; 

    // Generates a unique file path based on the slot number
    private static string GetPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");
    }

    // Checks if a specific slot has a save file (useful for UI button states)
    public static bool HasSave(int slot)
    {
        return File.Exists(GetPath(slot));
    }

    // --- SAVE OPERATIONS ---
    public static void Save(SaveData data)
    {
        SaveToSlot(data, CurrentSlot);
    }

    // 2. For saving at checkpoints
    public static void SaveToSlot(SaveData data, int slot)
    {
        CurrentSlot = slot; // Update the active slot
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(slot), json);
    }

    // --- LOAD OPERATIONS ---

    // 1. For in-game quick loads (e.g., respawning after death in the current slot)
    public static SaveData Load()
    {
        return LoadFromSlot(CurrentSlot);
    }

    // 2. For loading a specific slot from the UI
    public static SaveData LoadFromSlot(int slot)
    {
        CurrentSlot = slot; // Set the chosen slot as the active one
        string path = GetPath(slot);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        
        Debug.LogWarning($"Slot {slot} is empty. Creating new save data.");
        return new SaveData(); 
    }

    // For a future "Delete Save" (Trash Bin) button in the UI
    public static void DeleteSave(int slot)
    {
        string path = GetPath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Slot {slot} successfully deleted.");
        }
    }
}