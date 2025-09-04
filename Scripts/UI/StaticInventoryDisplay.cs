using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.Events;
using System.Linq;

public class StaticInventoryDisplay : InventoryDisplay 
{
    [SerializeField] private InventoryHolder inventoryHolder; // Reference to the inventory holder
    [SerializeField] private InventorySlotUI[] slots; // Array of inventory slot UI elements

    private void OnEnable()
    {
        PlayerInventory.OnPlayerInventoryChanged += RefreshStaticInventoryDisplay;
    }

    private void OnDisable()
    {
        PlayerInventory.OnPlayerInventoryChanged -= RefreshStaticInventoryDisplay;
    }

    private void RefreshStaticInventoryDisplay()
    {
        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.InventorySystem;
            inventorySystem.OnSlotChanged += UpdateSlot;
            Debug.Log($"InventorySystem initialized with {inventorySystem.InventorySize} slots.");

        }
        else // InventoryHolder reference is missing
        {
            Debug.LogWarning($"InventoryHolder reference is missing in {this.gameObject}");
        }

        AssignSlot(InventorySystem, 0);
    }

    protected override void Start() // Initialize the inventory display
    {
        base.Start();
        RefreshStaticInventoryDisplay();
    }

        public override void AssignSlot(InventorySystem invToDisplay, int offset) // Assign the inventory system to display
    {

        slotUIMap = new Dictionary<InventorySlotUI, InventorySlot>();
        for (int i = 0; i < inventoryHolder.Offset; i++)
        {
            if (i >= slots.Length)
            {
                Debug.LogError($"Slot index {i} exceeds the number of assigned InventorySlotUI objects.");
                continue;
            }

            slotUIMap.Add(slots[i], InventorySystem.InventorySlots[i]);
            slots[i].Init(InventorySystem.InventorySlots[i]);
            Debug.Log($"Assigned InventorySlot {i} to InventorySlotUI {slots[i].name}");
        }
    }
}
