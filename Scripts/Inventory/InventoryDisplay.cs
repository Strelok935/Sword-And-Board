using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] private InputActionReference splitStackAction; // Reference to the Input Action for splitting stacks
    [SerializeField] MouseItemData mouseItemData; // The mouse item data
    [SerializeField] private PlayerItemEffectController itemEffectController;
    protected InventorySystem inventorySystem; // The inventory system
    protected Dictionary<InventorySlotUI, InventorySlot> slotUIMap; // Mapping of InventorySlotUI to InventorySlot

    public InventorySystem InventorySystem => inventorySystem; // The inventory system
    public Dictionary<InventorySlotUI, InventorySlot> SlotUIMap => slotUIMap; // Mapping of InventorySlotUI to InventorySlot

    protected virtual void Start() // Initialize the inventory split input
    {
        if (splitStackAction != null)
        {
            splitStackAction.action.Enable();
        }   

        if (itemEffectController == null)
        {
            itemEffectController = FindObjectOfType<PlayerItemEffectController>();
        }

    }

    public abstract void AssignSlot(InventorySystem invToDisplay, int offset); // Assign the inventory system to display

    protected virtual void UpdateSlot(InventorySlot updatedSlot) // Update the UI for the given inventory slot
    {
        foreach (var slot in slotUIMap) // Iterate through the slot UI map
        {
            if (slot.Value == updatedSlot) // Slot Value - InventorySlotUI
            {
                slot.Key.UpdateUISlot(updatedSlot); // Slot Key - InventorySlot - UI Representation
            }
        }
    }

    public void SlotClicked(InventorySlotUI clickedUISlot) // Handle slot click events
    {
        bool isSplitStackPressed = splitStackAction != null && splitStackAction.action.IsPressed(); // Check if the split stack action is pressed

        if (mouseItemData == null)
        {
            Debug.LogError("MouseItemData is not assigned in InventoryDisplay.");
            return;
        }

        if (clickedUISlot.AssignedInventorySlot == null)
        {
            Debug.LogError("Clicked InventorySlotUI does not have an assigned InventorySlot.");
            return;
        }

        // Clicked Slot Has Item - Mouse does not have item - Pick up the slot item
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseItemData.AssignedInventorySlot?.ItemData == null)
        {
            //If Shift Key is Pressed, Split the Stack
            if (isSplitStackPressed && clickedUISlot.AssignedInventorySlot.SplitStack(out InventorySlot halfStackSlot))
            {
                mouseItemData.UpdateMouseSlot(halfStackSlot);
                clickedUISlot.UpdateUISlot();
                return;
            }
            else
            {
                mouseItemData.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
                clickedUISlot.ClearSlot();
                return;
            }

        }

        // Clicked Slot Does not have item - Mouse Does have item - Place Item into empty slot

        if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseItemData.AssignedInventorySlot?.ItemData != null)
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(mouseItemData.AssignedInventorySlot);
            clickedUISlot.UpdateUISlot();
            mouseItemData.ClearSlot(); // Clear the mouse slot after placing the item
            return;
        }

        // Clicked Slot has item - Mouse Has item - Decide What to do
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseItemData.AssignedInventorySlot?.ItemData != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseItemData.AssignedInventorySlot.ItemData;

            if (isSameItem && clickedUISlot.AssignedInventorySlot.RoomLeftInStack(mouseItemData.AssignedInventorySlot.StackSize))
            {
                // If the clicked slot has the same item and room in stack, add to stack
                clickedUISlot.AssignedInventorySlot.AssignItem(mouseItemData.AssignedInventorySlot);
                clickedUISlot.UpdateUISlot();
                mouseItemData.ClearSlot(); // Clear the mouse slot after adding to stack

            }
            else if (isSameItem && !clickedUISlot.AssignedInventorySlot.RoomLeftInStack(mouseItemData.AssignedInventorySlot.StackSize, out int leftInStack)) // Check if there's room left in the stack
            {
                if (leftInStack < 1) SwapSlots(clickedUISlot);
                else
                {
                    int remainingOnMouse = mouseItemData.AssignedInventorySlot.StackSize - leftInStack;
                    clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                    clickedUISlot.UpdateUISlot();

                    var NewItem = new InventorySlot(mouseItemData.AssignedInventorySlot.ItemData, remainingOnMouse);
                    mouseItemData.ClearSlot();
                    mouseItemData.UpdateMouseSlot(NewItem);
                    return;

                }

            }
            else if (!isSameItem) // If the clicked slot has a different item
            {
                SwapSlots(clickedUISlot);
                return;

            }
            
        }
    }

    public void SlotDoubleClicked(InventorySlotUI clickedUISlot) // Handle double-click use events
    {
        if (mouseItemData != null && mouseItemData.AssignedInventorySlot?.ItemData != null)
        {
            return;
        }

        if (clickedUISlot == null || clickedUISlot.AssignedInventorySlot?.ItemData == null)
        {
            return;
        }

        if (itemEffectController == null)
        {
            itemEffectController = FindObjectOfType<PlayerItemEffectController>();
        }

        if (itemEffectController == null)
        {
            Debug.LogWarning("No PlayerItemEffectController found to use inventory items.");
            return;
        }

        var itemData = clickedUISlot.AssignedInventorySlot.ItemData;
        if (!itemEffectController.TryApplyItem(itemData))
        {
            return;
        }

        clickedUISlot.AssignedInventorySlot.RemoveFromStack(1);
        clickedUISlot.UpdateUISlot();
    }

    private void SwapSlots(InventorySlotUI clickedUISlot) // Swap the contents of the clicked slot with the mouse slot
    {
        if (mouseItemData.AssignedInventorySlot == null || clickedUISlot.AssignedInventorySlot == null)
        {
            Debug.LogError("Cannot swap slots because one of the slots is null.");
            return;
        }

        // Create a temporary clone of the mouse slot's inventory
        var tempSlot = new InventorySlot(mouseItemData.AssignedInventorySlot.ItemData, mouseItemData.AssignedInventorySlot.StackSize);

        // Update the mouse slot with the clicked slot's inventory
        mouseItemData.ClearSlot();
        mouseItemData.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);

        // Update the clicked slot with the temporary slot's inventory
        clickedUISlot.ClearSlot();
        clickedUISlot.AssignedInventorySlot.AssignItem(tempSlot);
        clickedUISlot.UpdateUISlot();
    }
}
