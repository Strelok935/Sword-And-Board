using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System.IO;

public class SaveLoad : MonoBehaviour
{
    public static UnityAction OnSaveGame;
    public static UnityAction<SaveData> OnLoadGame;


    private static string saveFilePath = "/SaveData/";
    private static string saveFileName = "savefile.json";

    public static bool Save(SaveData data)
    {
        OnSaveGame?.Invoke();
        string dir = Application.persistentDataPath + saveFilePath;
        GUIUtility.systemCopyBuffer = dir;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(dir + saveFileName, json);
        Debug.Log("Game Saved to " + dir + saveFileName);
        return true;
    }

    public static SaveData Load()
    {
        string dir = Application.persistentDataPath + saveFilePath + saveFileName;
        SaveData data = new SaveData();
        if (File.Exists(dir))
        {
            string json = File.ReadAllText(dir);
            data = JsonUtility.FromJson<SaveData>(json);
            OnLoadGame?.Invoke(data);
            
        }
        else
        {
            Debug.Log("No save file found at " + dir);
        }
        Debug.Log("Loaded game data from " + dir);
        return data;
    }

    public static void DeleteSaveData()
    {
        string dir = Application.persistentDataPath + saveFilePath + saveFileName;
        if (File.Exists(dir))
        {
            File.Delete(dir);
            Debug.Log("Save file deleted at " + dir);
        }
        else
        {
            Debug.Log("No save file found to delete at " + dir);
        }
    }

}
