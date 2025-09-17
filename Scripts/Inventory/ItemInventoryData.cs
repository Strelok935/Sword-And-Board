using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]


public class ItemInventoryData : ScriptableObject
{
    public int itemID = -1; // Unique identifier for the item;
    public string itemName; // Name of the item;
    public bool isEquippable; // Equipment Value;
    public bool isUniversal; // Universal Value;
    public Sprite icon; // Icon to display in the inventory;
    public bool isStackable; // Can this item stack?;
    public int maxStackSize = 64; // Maximum stack size (if stackable);
    [TextArea(4, 4)]
    public string Description; // Description of the item;

    public int MoneyValue; // Value of the item in game currency;

    public string ItemType; // Type of the item (e.g., Weapon, Armor, Consumable);
    public GameObject WorldModelPrefab; // Prefab to represent the item in the world;



}

