using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using System;



public class SaveData
{
    public int playerLevel;
    public float health;
    public float stamina;
    public float money;
    public Vector3 position;
    public List<string> inventoryItems;
    public List<string> collectedItems;
    public SerializableDictionary<string, ItemPickUpSaveData> activeItems;
    public SerializableDictionary<string, SaveDataInventory> chestDictionary;

    public SaveDataInventory playerInventory;
    public SaveData()
    {
        playerLevel = 1;
        health = 100f;
        money = 0f;
        position = Vector3.zero;
        inventoryItems = new List<string>();
        collectedItems = new List<string>();
        activeItems = new SerializableDictionary<string, ItemPickUpSaveData>();
        chestDictionary = new SerializableDictionary<string, SaveDataInventory>();
        playerInventory = new SaveDataInventory();
    }
}