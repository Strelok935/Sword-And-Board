using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class InventorySystem
{
    [SerializeField] private List<InventorySlot> inventorySlots;

    public List<InventorySlot> InventorySlots => inventorySlots;
    public int InventorySize => inventorySlots.Count;
    public UnityAction<InventorySlot> OnSlotChanged;
    public InventorySystem(int size)
    {
        inventorySlots = new List<InventorySlot>(size);
        for (int i = 0; i < size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    public bool AddToInventory(InventoryItemData itemToAdd, int amountToAdd)
    {
        if (ContainsItem(itemToAdd, out List<InventorySlot> invSlot)) // Check Whether the item already exists
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

            if (HasFreeSlot(out InventorySlot freeSlot)) // Gets the first free slot
            {
                freeSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
                OnSlotChanged?.Invoke(freeSlot);
                return true;
            }

            return false; // No free slot and no room in existing stacks
        }

        // If the item is not already in the inventory
        if (HasFreeSlot(out InventorySlot newSlot))
        {
            newSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
            OnSlotChanged?.Invoke(newSlot);
            return true;
        }

        return false; // No free slot available
    }

    public bool ContainsItem(InventoryItemData itemToAdd, out List<InventorySlot> invSlot)
    {
        invSlot = inventorySlots.Where(i => i.ItemData == itemToAdd).ToList();
        return invSlot.Count > 1 ? true : false;

    }

    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = inventorySlots.FirstOrDefault(i => i.ItemData == null);
        return freeSlot == null ? false : true;
    }

}
