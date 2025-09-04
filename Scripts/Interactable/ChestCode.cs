using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;


[RequireComponent(typeof(IdSystem))]

public class ChestCode : InventoryHolder, IInteractable
{
    public UnityAction<IInteractable> OnInteract { get; set; }

    protected override void Awake()
    {
        base.Awake();
        SaveLoad.OnLoadGame += LoadInventory;
    }

    private void Start()
    {
        var chestSaveData = new SaveDataInventory(primaryInventorySystem, transform.position, transform.rotation);

        SaveGameManager.data.chestDictionary.Add(GetComponent<IdSystem>().Id, chestSaveData);
    }

    protected override void LoadInventory(SaveData data)
    {
        if (data.chestDictionary.TryGetValue(GetComponent<IdSystem>().Id, out SaveDataInventory chestSaveData))
        {
            this.primaryInventorySystem = chestSaveData.InvSyst;
            this.transform.position = chestSaveData.Position;
            this.transform.rotation = chestSaveData.Rotation;
        }
    }

    public void Interact(Interactor interactor, out bool interactionSuccess)
    {
        OnDynamicInventoryDisplayRequested?.Invoke(primaryInventorySystem, 0);
        interactionSuccess = true;
    }

    public void StopInteract()
    {
        // Optional: Handle stopping interaction if needed
    }

}


