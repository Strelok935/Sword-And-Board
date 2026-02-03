using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChestStartItem
{
    public ItemInventoryData item;
    public int amount = 1;
}

public class ChestStartInventory : MonoBehaviour
{
    [SerializeField] private ChestCode chest;
    [SerializeField] private List<ChestStartItem> startingItems = new List<ChestStartItem>();

    private bool initialized = false;

    private void Start()
    {
        if (SaveGameManager.IsLoadingGame) return; // ← prevents refill on load


        if (initialized) return;

        if (chest == null)
        {
            Debug.LogError("Chest reference missing on ChestStartInventory");
            return;
        }

        var inventory = chest.InventorySystem; // Your getter from InventoryHolder

        if (inventory == null)
        {
            Debug.LogError("Inventory system not ready");
            return;
        }

        FillInventory(chest.InventorySystem);
        initialized = true;
    }

    private void FillInventory(InventorySystem inventory)
    {
        foreach (var entry in startingItems)
        {
            if (entry.item == null || entry.amount <= 0)
                continue;

            bool added = inventory.AddToInventory(entry.item, entry.amount);

            if (!added)
                Debug.LogWarning($"Chest could not fit all of {entry.item.itemName}");
        }
    }
}
