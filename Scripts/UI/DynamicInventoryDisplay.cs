using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField] protected InventorySlotUI slotUIPrefab;
    protected override void Start() // Initialize the inventory display
    {
        // InventoryHolder.OnDynamicInventoryDisplayRequested += RefreshDynamicInventoryDisplay;
        base.Start();

        // AssignSlot(inventorySystem);
    }

    protected void OnDestroy()
    {
        // InventoryHolder.OnDynamicInventoryDisplayRequested -= RefreshDynamicInventoryDisplay;
    }


    public void RefreshDynamicInventoryDisplay(InventorySystem invToDisplay, int offset)
    {
        ClearSlots();
        inventorySystem = invToDisplay;
        if (inventorySystem != null) inventorySystem.OnSlotChanged += UpdateSlot;
        AssignSlot(invToDisplay, offset);
    }

    public override void AssignSlot(InventorySystem invToDisplay, int offset) // Assign the inventory system to display
    {
        ClearSlots();

        slotUIMap = new Dictionary<InventorySlotUI, InventorySlot>();
        if (invToDisplay == null) return;



        for (int i = offset; i < invToDisplay.InventorySize; i++)
        {
            var slotUI = Instantiate(slotUIPrefab, transform);
            slotUIMap.Add(slotUI, invToDisplay.InventorySlots[i]);
            slotUI.Init(invToDisplay.InventorySlots[i]);
            slotUI.UpdateUISlot();

        }
    }

    private void ClearSlots()
    {
        foreach (var item in transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }

        if (slotUIMap != null) slotUIMap.Clear();
    }
    
    private void OnDisable()
    {
        if (inventorySystem != null) inventorySystem.OnSlotChanged -= UpdateSlot;
    }
}
