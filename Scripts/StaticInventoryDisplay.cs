using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.Events;
using System.Linq;

public class StaticInventoryDisplay : InventoryDisplay  
{
    [SerializeField] private InventoryHolder inventoryHolder;
    [SerializeField] private InventorySlotUI[] slots;

    protected override void Start()
    {
        base.Start();

        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.InventorySystem;
            inventorySystem.OnSlotChanged += UpdateSlot;
            Debug.Log($"InventorySystem initialized with {inventorySystem.InventorySize} slots.");

        }
        else
        {
            Debug.LogWarning($"InventoryHolder reference is missing in {this.gameObject}");
        }

        AssignSlot(InventorySystem);
    }

        public override void AssignSlot(InventorySystem invToDisplay)
    {
        if (slots.Length != InventorySystem.InventorySize)
        {
            Debug.LogWarning($"Inventory size mismatch. Inventory Size: {invToDisplay.InventorySize}, Slot UIs assigned: {slots.Length}, on {this.gameObject}");
        }

        slotUIMap = new Dictionary<InventorySlotUI, InventorySlot>();
        for (int i = 0; i < InventorySystem.InventorySize; i++)
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
