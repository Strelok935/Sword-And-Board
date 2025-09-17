using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
[CreateAssetMenu(fileName = "Shop System", menuName = "Shop/Item")]
public class ShopItems : ScriptableObject
{
    [SerializeField] private List<ShopItemInventory> _items;
    [SerializeField] private int _traderGold;
    [SerializeField] private float _markupSell;
    [SerializeField] private float _markupBuy;

    public List<ShopItemInventory> Items => _items;
    public int MaxAllowedGold => _traderGold;
    public float MarkupSell => _markupSell;
    public float MarkupBuy => _markupBuy;



}

[System.Serializable]

public struct ShopItemInventory
{
    public ItemInventoryData ItemData;
    public int quantity;

}
