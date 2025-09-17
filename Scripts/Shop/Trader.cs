using System.Security.AccessControl;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(IdSystem))]
public class Trader : MonoBehaviour, IInteractable
{
    [SerializeField] private ShopItems _shopItems;
    [SerializeField] private TradeSystem _shopSystem;

    public static UnityAction<TradeSystem, PlayerInventory> OnOpenTrader;

    private string _id;

    private ShopSaveData _shopSaveData;


    private void Awake()
    {
        _shopSystem =
        new TradeSystem
        (_shopItems.Items.Count, _shopItems.MaxAllowedGold, _shopItems.MarkupBuy, _shopItems.MarkupSell);

        foreach (var item in _shopItems.Items)
        {
            Debug.Log("Adding item to shop: " + item.ItemData.itemName + " Quantity: " + item.quantity);
            _shopSystem.AddToShop(item.ItemData, item.quantity);
        }

        _id = GetComponent<IdSystem>().Id;
        _shopSaveData = new ShopSaveData(_shopSystem);
    }


    private void Start()
    {
        if (!SaveGameManager.data.traderDictionary.ContainsKey(_id)) SaveGameManager.data.traderDictionary.Add(_id, _shopSaveData);
    }

    private void  OnEnable()
    {
        SaveLoad.OnLoadGame += LoadInventory;
    }

    private void LoadInventory(SaveData data)
    {
        if (data.traderDictionary.TryGetValue(_id, out ShopSaveData shopSaveData)) return;

        _shopSaveData = shopSaveData;
        _shopSystem = _shopSaveData.TradeSystem; 
    }

    private void OnDisable()
    {
        SaveLoad.OnLoadGame -= LoadInventory;
    }

    public UnityAction<IInteractable> OnInteract { get; set; } // Required by IInteractable
    public void Interact(Interactor interactor, out bool interactionSuccess)
    {

        var playerInventory = interactor.GetComponent<PlayerInventory>();

        if (playerInventory != null)
        {

            OnOpenTrader?.Invoke(_shopSystem, playerInventory);
            interactionSuccess = true;
        }
        else
        {
            interactionSuccess = false;
            Debug.Log("No Player Inventory found on Interactor");
        }
    }

    public void StopInteract()
    {
        // Optional: Add logic if needed when interaction stops
    }

}

[System.Serializable]
public class ShopSaveData
{
    public TradeSystem TradeSystem;

    public ShopSaveData(TradeSystem tradeSystem)
    {
        TradeSystem = tradeSystem;
    }
}
