using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Runtime.Serialization;

public abstract class ItemSlot : ISerializationCallbackReceiver
{
    [NonSerialized] public ItemInventoryData itemData; // The data for the item in this slot
    [SerializeField] protected int _itemID = -1; // Item ID for serialization
    [SerializeField] protected int stackSize; // stack size

    public ItemInventoryData ItemData => itemData;
    public int StackSize => stackSize;

    public ItemSlot() // Default constructor
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


    public void AssignItem(ItemInventoryData data, int amount)
    {
        if (itemData == data) AddToStack(amount); // Same item
        else // Different item
        {
            itemData = data;
            _itemID = itemData.itemID;
            stackSize = 0;
            AddToStack(amount);
        }
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



    public void UpdateItemSlot(ItemInventoryData itemData, int stackSize) // Update the item slot with new item data and stack size
    {
        this.itemData = itemData;
        this.stackSize = stackSize;
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
