using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Linq;


[System.Serializable]
public class TradeSystem
{
    [SerializeField] private List<ShopSlot> _shopInventory;
    [SerializeField] private int _availableGold;
    [SerializeField] private float _buyMarkup;
    [SerializeField] private float _sellMarkup;

    public List<ShopSlot> ShopInventory => _shopInventory;

    public int traderGold => _availableGold;

    public float BuyMarkUp => _buyMarkup;

    public float SellMarkUp => _sellMarkup;


    public TradeSystem(int size, int currency, float buyMarkup, float sellMarkup)
    {
        _availableGold = currency;
        _buyMarkup = buyMarkup;
        _sellMarkup = sellMarkup;
        _availableGold = traderGold;

        ShopSize(size);
    }

    private void ShopSize(int size)
    {
        _shopInventory = new List<ShopSlot>(size);

        for (int i = 0; i < size; i++)
        {
            _shopInventory.Add(new ShopSlot());
        }
    }

    public void AddToShop(ItemInventoryData itemData, int amount)
    {
        if (ContainsItem(itemData, out ShopSlot shopSlot))
        {
            shopSlot.AddToStack(amount);
            return;
        }


        var freeSlot = GetFreeSlot();
        freeSlot.AssignItem(itemData, amount);
    }

    private ShopSlot GetFreeSlot()
    {
        var freeSlot = _shopInventory.FirstOrDefault(i => i.ItemData == null);

        if (freeSlot == null)
        {
            freeSlot = new ShopSlot();
            _shopInventory.Add(freeSlot);
        }

        return freeSlot;
    }

    public bool ContainsItem(ItemInventoryData itemToAdd, out ShopSlot shopSlot) // Check if the Shop contains the item
    {
        shopSlot = _shopInventory.Find(i => i.ItemData == itemToAdd);
        return shopSlot != null;

    }

    public void PurchaseItem(ItemInventoryData data, int amount)
    {
        if (!ContainsItem(data, out ShopSlot slot)) return;

        slot.RemoveFromStack(amount);
    }

    public void GainGold(int basketTotal)
    {
        _availableGold += basketTotal;
    }

    public void SellItem(ItemInventoryData kvpKey, int kvpValue, int price)
    {
        AddToShop(kvpKey, kvpValue); // Add the sold item to the shop's inventory
        ReduceGold(price); // Deduct the price from the trader's gold

        Debug.Log($"Item {kvpKey.itemName} sold to shop. Quantity: {kvpValue}. Shop now has {kvpKey.itemName} in inventory.");
    }

    private void ReduceGold(int price)
    {
        _availableGold -= price;
    }

}
