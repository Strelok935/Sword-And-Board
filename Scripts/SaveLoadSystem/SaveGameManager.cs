using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using System;


public class SaveGameManager : MonoBehaviour
{
    public static SaveData data;
    public static bool IsLoadingGame { get; private set; }


    private void Awake()
    {
        data = new SaveData();
        SaveLoad.OnLoadGame += LoadGame;
    }

    public void DeleteData()
    {
        SaveLoad.DeleteSaveData();
    }

    public static void SaveData()
    {
        var saveData = data;

        SaveLoad.Save(saveData);
    }

        public static void LoadGame(SaveData _data)
    {
        IsLoadingGame = true;
        data = _data;
    }
    public static void StartNewGame()
    {
        data = new SaveData();
        IsLoadingGame = false;
    }



    public static void LoadData()
    {
        SaveLoad.Load();
    }

}
