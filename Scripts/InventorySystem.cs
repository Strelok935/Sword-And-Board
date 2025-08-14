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

    public bool AddToInventory(ItemData itemData, int amount = 1)
    {
        if (itemData == null || amount <= 0) return false;

        // Check for existing stack
        foreach (var slot in inventorySlots)
        {
            if (slot.ItemData == itemData && slot.RoomLeftInStack(amount))
            {
                slot.AddToStack(amount);
                OnSlotChanged?.Invoke(slot);
                return true;
            }
        }

        // Find an empty slot or a slot with room
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].ItemData == null || inventorySlots[i].RoomLeftInStack(amount))
            {
                inventorySlots[i] = new InventorySlot(itemData, amount);
                OnSlotChanged?.Invoke(inventorySlots[i]);
                return true;
            }
        }

        return false; // Inventory is full
    }

}
