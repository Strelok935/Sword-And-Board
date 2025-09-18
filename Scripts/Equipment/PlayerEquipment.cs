// 9/17/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerEquipment : MonoBehaviour
{
    public static UnityAction OnPlayerEquipmentChanged; // Event triggered when equipment changes
    public static UnityAction<EquipmentSystem, int> OnPlayerEquipmentDisplayRequested; // Event to request equipment display

    [SerializeField] private DynamicEquipmentDisplay equipmentDisplay; // Reference to the equipment display
    private EquipmentSystem equipmentSystem; // The equipment system

    private void Start()
    {
        // Initialize the equipment system with 10 slots
        equipmentSystem = new EquipmentSystem(10);

        // Initialize the equipment display with the equipment system
        equipmentDisplay.Initialize(equipmentSystem);

    }

    private void Update()
    {
        // Example: Open the equipment UI when the "E" key is pressed
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            OnPlayerEquipmentDisplayRequested?.Invoke(equipmentSystem, 0);
        }
    }

    public bool AddToEquipment(ItemInventoryData data, int amount)
    {
        // Ensure the item is equippable before adding it to the equipment system
        if (data.isEquippable && equipmentSystem.AddToEquipment(data, amount))
        {
            OnPlayerEquipmentChanged?.Invoke(); // Notify listeners of equipment changes
            return true;
        }

        Debug.LogWarning($"Item {data.itemName} is not equippable or could not be added to equipment.");
        return false;
    }
}
