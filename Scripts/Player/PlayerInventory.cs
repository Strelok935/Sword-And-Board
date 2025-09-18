using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerInventory : InventoryHolder
{
    
    public static UnityAction OnPlayerInventoryChanged;
    
    public static UnityAction<InventorySystem, int> OnPlayerInventoryDisplayRequested;
    [SerializeField] private DynamicEquipmentDisplay equipmentDisplay;
    private EquipmentSystem equipmentSystem;


    public void Start()
    {
        SaveGameManager.data.playerInventory = new SaveDataInventory(primaryInventorySystem);
        equipmentSystem = new EquipmentSystem(10); // Create an equipment system with 10 slots
        equipmentDisplay.Initialize(equipmentSystem); // Initialize the display

    }
 


    protected override void LoadInventory(SaveData data)
    {
        if (data.playerInventory.InvSyst != null)
        {
            this.primaryInventorySystem = data.playerInventory.InvSyst;
            OnPlayerInventoryChanged?.Invoke();
        }
    }

    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            OnPlayerInventoryDisplayRequested?.Invoke(primaryInventorySystem, offset);
        }
    }

    public bool AddToInventory(ItemInventoryData data, int amount)
    {
        if(primaryInventorySystem.AddToInventory(data, amount))
        {
            return true;
        }
        return false;
    }

}
