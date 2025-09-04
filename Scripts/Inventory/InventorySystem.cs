using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class InventorySystem
{
    [SerializeField] private List<InventorySlot> inventorySlots; // List of inventory slots

    public List<InventorySlot> InventorySlots => inventorySlots; // Get the list of inventory slots
    public int InventorySize => inventorySlots.Count; // Get the size of the inventory
    public UnityAction<InventorySlot> OnSlotChanged; // Callback invoked when a slot changes
    public InventorySystem(int size) // Constructor for the inventory system
    {
        inventorySlots = new List<InventorySlot>(size);
        for (int i = 0; i < size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

     public bool AddToInventory(ItemInventoryData itemToAdd, int amountToAdd) // Add an item to the inventory
    {
        if (itemToAdd.isStackable) // Handle stackable items
        {
            if (ContainsItem(itemToAdd, out List<InventorySlot> invSlot)) // Check whether the item already exists
            {
                foreach (var slot in inventorySlots)
                {
                    if (slot.ItemData == itemToAdd && slot.RoomLeftInStack(amountToAdd))
                    {
                        slot.AddToStack(amountToAdd);
                        OnSlotChanged?.Invoke(slot);
                        return true;
                    }
                }
            }

            if (HasFreeSlot(out InventorySlot freeSlot)) // Get the first free slot
            {
                freeSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
                OnSlotChanged?.Invoke(freeSlot);
                return true;
            }

            return false; // No free slot and no room in existing stacks
        }
        else // Handle non-stackable items
        {
            for (int i = 0; i < amountToAdd; i++) // Add each item individually
            {
                if (HasFreeSlot(out InventorySlot freeSlot))
                {
                    freeSlot.UpdateInventorySlot(itemToAdd, 1); // Add one item per slot
                    OnSlotChanged?.Invoke(freeSlot);
                }
                else
                {
                    Debug.LogWarning("Inventory is full! Could not add item: " + itemToAdd.itemName);
                    return false; // No free slot available
                }
            }

            return true; // Successfully added all items
        }
    }
    public bool ContainsItem(ItemInventoryData itemToAdd, out List<InventorySlot> invSlot) // Check if the inventory contains the item
    {
        invSlot = inventorySlots.Where(i => i.ItemData == itemToAdd).ToList();
        return invSlot.Count > 0 ? true : false;

    }

    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = inventorySlots.FirstOrDefault(i => i.ItemData == null);
        return freeSlot == null ? false : true;
    }

}
