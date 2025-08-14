using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;
public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] MouseItemData mouseItemData;
    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlotUI, InventorySlot> slotUIMap;

    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlotUI, InventorySlot> SlotUIMap => slotUIMap;

    protected virtual void Start()
    {
        
    }

    public abstract void AssignSlot(InventorySystem invToDisplay);

    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        foreach (var slot in slotUIMap)
        {
            if (slot.Value == updatedSlot) // Slot Value - InventorySlotUI
            {
                slot.Key.UpdateUISlot(updatedSlot); // Slot Key - InventorySlot - UI Representation
            }
        }
    }

    public void SlotClicked(InventorySlotUI clickedSlot)
    {
        Debug.Log($"Slot clicked: {clickedSlot}");
    }
}
