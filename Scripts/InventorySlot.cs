using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class InventorySlot
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int stackSize;

    public ItemData ItemData => itemData;
    public int StackSize => stackSize;

    public InventorySlot(ItemData itemData, int stackSize)
    {
        this.itemData = itemData;
        this.stackSize = stackSize;

    }

    public InventorySlot()
    {
        ClearSlot();
    }

    public void ClearSlot()
    {
        itemData = null;
        stackSize = -1;
    }
    
    public void UpdateInventorySlot(ItemData itemData, int stackSize)
    {
        this.itemData = itemData;
        this.stackSize = stackSize;
    }


    public bool RoomLeftInStack(int amountToAdd, out int amountRemaining)
    {
        amountRemaining = ItemData.maxStackSize - stackSize;
        return RoomLeftInStack(amountToAdd);
    }

    public bool RoomLeftInStack(int amountToAdd)
    {
        if (stackSize + amountToAdd <= itemData.maxStackSize)
        {
            return true;
        }
        return false;
    }

    public bool AddToStack(int amount)
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
    public bool RemoveFromStack(int amount)
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

}
