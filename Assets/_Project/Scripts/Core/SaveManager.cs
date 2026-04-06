using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string path = Path.Combine(Application.persistentDataPath, "savefile.json");
    
    // Check if a save file exists
    public static bool HasSave()
    {
        return File.Exists(path);
    }

    // Save data to JSON file
    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    //Load data from JSON file
    public static SaveData Load()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return new SaveData(); // Return new data if no save file exists (New Game)
    }
}