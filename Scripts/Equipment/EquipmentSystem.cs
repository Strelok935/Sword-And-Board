using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class EquipmentSystem
{
    [SerializeField] private List<InventorySlot> equipmentSlots; // List of equipment slots

    public List<InventorySlot> EquipmentSlots => equipmentSlots; // Get the list of equipment slots
    public int EquipmentSize => equipmentSlots.Count; // Get the size of the equipment inventory

    public EquipmentSystem(int size) // Constructor for the equipment system
    {
        CreateEquipment(size);
    }

    private void CreateEquipment(int size)
    {
        equipmentSlots = new List<InventorySlot>(size);
        for (int i = 0; i < size; i++)
        {
            equipmentSlots.Add(new InventorySlot());
        }
    }

    public bool AddToEquipment(ItemInventoryData itemToAdd, int amountToAdd) // Add an item to the equipment system
    {
        if (!itemToAdd.isEquippable) // Ensure the item is equippable
        {
            Debug.LogWarning($"Item {itemToAdd.itemName} is not equippable!");
            return false;
        }

        // Check for an empty slot in the equipment system
        foreach (var slot in equipmentSlots)
        {
            if (slot.ItemData == null) // Empty slot found
            {
                slot.UpdateInventorySlot(itemToAdd, amountToAdd);
                return true; // Successfully added the item
            }
        }

        Debug.LogWarning("No empty equipment slot available!");
        return false; // No empty slot available
    }
}