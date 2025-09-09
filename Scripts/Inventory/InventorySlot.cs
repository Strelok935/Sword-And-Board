using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Runtime.Serialization;

[System.Serializable]
public class InventorySlot : ISerializationCallbackReceiver
{
    [NonSerialized] public ItemInventoryData itemData; // The data for the item in this slot
    [SerializeField] private int _itemID = -1; // Item ID for serialization
    [SerializeField] private int stackSize; // stack size

    public ItemInventoryData ItemData => itemData;
    public int StackSize => stackSize;

    public InventorySlot(ItemInventoryData itemData, int stackSize) // Constructor for creating a new inventory slot
    {
        this.itemData = itemData;
        _itemID = itemData.itemID;
        this.stackSize = stackSize;

    }

    public InventorySlot() // Default constructor
    {
        ClearSlot();
    }

    public void ClearSlot()
    {
        itemData = null;
        _itemID = -1;
        stackSize = -1;
    }

    public void AssignItem(InventorySlot invSlot) // Assign an item from another inventory slot
    {
        if (itemData == invSlot.ItemData) AddToStack(invSlot.stackSize); // Same item
        else // Different item
        {
            itemData = invSlot.itemData;
            _itemID = itemData.itemID;
            stackSize = 0;
            AddToStack(invSlot.stackSize);
        }
    }

    public void UpdateInventorySlot(ItemInventoryData itemData, int stackSize) // Update the inventory slot with new item data and stack size
    {
        this.itemData = itemData;
        _itemID = itemData.itemID;
        this.stackSize = stackSize;
    }


    public bool RoomLeftInStack(int amountToAdd, out int amountRemaining) // Check if there is room left in the stack
    {
        amountRemaining = ItemData.maxStackSize - stackSize;
        return RoomLeftInStack(amountToAdd);
    }

    public bool RoomLeftInStack(int amountToAdd) // Check if there is room left in the stack
    {
        if (stackSize + amountToAdd <= itemData.maxStackSize)
        {
            return true;
        }
        return false;
    }

    public bool AddToStack(int amount) // Add items to the stack
    {
        if (itemData == null || !itemData.isStackable)
            return false;

        stackSize += amount;
        if (stackSize > itemData.maxStackSize)
        {
            stackSize = itemData.maxStackSize;
            return false; // Stack overflow
        }
        return true; // Successfully added to stack
    }
    public bool RemoveFromStack(int amount) // Remove items from the stack
    {
        if (itemData == null || stackSize < amount)
            return false;

        stackSize -= amount;
        if (stackSize <= 0)
        {
            ClearSlot();
        }
        return true; // Successfully removed from stack
    }

    public bool SplitStack(out InventorySlot splitStack) // Split the stack into two
    {
        if (stackSize <= 1)
        {
            splitStack = null;
            return false; // Not enough items to split

        }

        int halfStack = Mathf.RoundToInt(stackSize / 2f);
        RemoveFromStack(halfStack);
        splitStack = new InventorySlot(itemData, halfStack);
        return true; // Successfully split the stack
    }

    public void OnBeforeSerialize()
    {
       // Implement if needed

    }

    public void OnAfterDeserialize()
    {

        if (_itemID == -1) return;

        var database = Resources.Load<Database>("Database");
        itemData = database.GetItemByID(_itemID);
    }
}
