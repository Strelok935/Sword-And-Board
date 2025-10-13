using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
public class DynamicEquipmentDisplay : InventoryDisplay
{
    [SerializeField] private InventorySlotUI slotUIPrefab; // Prefab for equipment slots
    [SerializeField] private List<Vector2> slotPositions; // List of predefined positions for the slots
    private EquipmentSystem equipmentSystem; // Reference to the equipment system
    private new Dictionary<InventorySlotUI, InventorySlot> slotUIMap; // Map of UI slots to equipment slots

    public void Initialize(EquipmentSystem equipmentSystem)
    {
        this.equipmentSystem = equipmentSystem;
        CreateSlots();
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

    private void CreateSlots()
    {
        ClearSlots();

        slotUIMap = new Dictionary<InventorySlotUI, InventorySlot>();

        for (int i = 0; i < equipmentSystem.EquipmentSize; i++)
        {
            var slotUI = Instantiate(slotUIPrefab, transform);

            // Assign the predefined position if available
            if (i < slotPositions.Count)
            {
                RectTransform slotRect = slotUI.GetComponent<RectTransform>();
                slotRect.anchoredPosition = slotPositions[i];
            }
            else
            {
                Debug.LogWarning($"No predefined position for slot {i}. Defaulting to (0, 0).");
            }

            slotUIMap.Add(slotUI, equipmentSystem.EquipmentSlots[i]);
            slotUI.Init(equipmentSystem.EquipmentSlots[i]);
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
