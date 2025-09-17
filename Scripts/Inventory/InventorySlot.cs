using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Runtime.Serialization;

[System.Serializable]
public class InventorySlot : ItemSlot
{


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

   
    
}
