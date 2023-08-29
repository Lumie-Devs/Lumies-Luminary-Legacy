using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    static string path = Application.persistentDataPath + "/savefile.json";

    public static void SaveGameData(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(path, json);
        Debug.Log("save");
    }

    public static GameData LoadGameData()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void DeleteSaveData()
    {
        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static bool SaveFileExists()
    {
        return File.Exists(path);
    }
}
