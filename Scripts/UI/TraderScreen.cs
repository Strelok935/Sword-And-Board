using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class TraderScreen : MonoBehaviour
{
    [SerializeField] private ShopSlotUI _shopSlotPrefab;
    [SerializeField] private ShopCartItemUI _cartSlotPrefab;
    [SerializeField] private Button _buyTab;
    [SerializeField] private Button _sellButton;

    [Header("Shopping Cart")]
    [SerializeField] private TextMeshProUGUI _cartTotalText;
    [SerializeField] private TextMeshProUGUI _playerGoldText;
    [SerializeField] private TextMeshProUGUI _traderGoldText;
    [SerializeField] private Button _buyButton;
    [SerializeField] private TextMeshProUGUI _buyButtonText;

    [Header("Item Preview")]
    [SerializeField] private Image _itemPreview;
    [SerializeField] private TextMeshProUGUI _itemPreviewName;
    [SerializeField] private TextMeshProUGUI _itemPreviewDescription;

    [SerializeField] private GameObject _itemListContent;
    [SerializeField] private GameObject _shoppingListContent;

    private bool _isSelling;

    private int _basketTotal;

    private TradeSystem _shopSystem;
    private PlayerInventory _playerInventory;

    private Dictionary<ItemInventoryData, int> _shoppingCart = new Dictionary<ItemInventoryData, int>();
    private Dictionary<ItemInventoryData, ShopCartItemUI> _shoppingCartUI =
            new Dictionary<ItemInventoryData, ShopCartItemUI>();

    public void DisplayTradeWindow(TradeSystem tradeSystem, PlayerInventory playerInventory)
    {
        _shopSystem = tradeSystem;
        _playerInventory = playerInventory;


        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (_buyButton != null)
        {
            _buyButtonText.text = _isSelling ? "Sell Items" : "Buy Items";
            _buyButton.onClick.RemoveAllListeners();
            if (_isSelling) _buyButton.onClick.AddListener(SellItems);
            else _buyButton.onClick.AddListener(BuyItems);
        }

        ClearSlot();
        ClearItemPreview();


        _cartTotalText.enabled = false;
        _buyButton.gameObject.SetActive(false);
        _basketTotal = 0;
        _playerGoldText.text = $"Player Gold: {_playerInventory.InventorySystem.Gold}";
        _traderGoldText.text = $"Shop Gold: {_shopSystem.traderGold}";

        if (_isSelling) DisplayPlayerInventory();
        else DisplayTraderInventory();

    }

    private void DisplayTraderInventory()
    {
        foreach (var item in _shopSystem.ShopInventory)
        {
            if (item.ItemData == null) continue;

            var ShopSlot = Instantiate(_shopSlotPrefab, _itemListContent.transform);
            ShopSlot.Init(item, _shopSystem.BuyMarkUp);

        }
    }

    private void BuyItems()
    {
        if (_playerInventory.InventorySystem.Gold < _basketTotal) return;
        if (!_playerInventory.InventorySystem.CheckInventoryRemain(_shoppingCart)) return;

        foreach (var kvp in _shoppingCart)
        {
            _shopSystem.PurchaseItem(kvp.Key, kvp.Value);

            for (int i = 0; i < kvp.Value; i++)
            {
                _playerInventory.InventorySystem.AddToInventory(kvp.Key, 1);
            }
        }

        _shopSystem.GainGold(_basketTotal);
        _playerInventory.InventorySystem.SpendGold(_basketTotal);

        RefreshDisplay();
    }
    private void SellItems()
    {
        if (_shopSystem.traderGold < _basketTotal) return;

        foreach (var kvp in _shoppingCart)
        {
            var price = GetModifiedPrice(kvp.Key, kvp.Value, _shopSystem.SellMarkUp);

            _shopSystem.SellItem(kvp.Key, kvp.Value, price);

            _playerInventory.InventorySystem.GainGold(price);
            _playerInventory.InventorySystem.RemoveItemsFromInventory(kvp.Key, kvp.Value);


        }

        RefreshDisplay();
    }

    private void ClearSlot()
    {
        _shoppingCart = new Dictionary<ItemInventoryData, int>();
        _shoppingCartUI = new Dictionary<ItemInventoryData, ShopCartItemUI>();

        foreach (var item in _itemListContent.transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }
        foreach (var item in _shoppingListContent.transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }
    }


    private void DisplayPlayerInventory()
    {
        foreach (var item in _playerInventory.InventorySystem.GetAllItemsHeld())
        {
            var tempSlot = new ShopSlot();
            tempSlot.AssignItem(item.Key, item.Value);

            var shopSlot = Instantiate(_shopSlotPrefab, _itemListContent.transform);
            shopSlot.Init(tempSlot, _shopSystem.SellMarkUp);
        }
    }

    public void RemoveItemFromCart(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;
        var price = GetModifiedPrice(data, 1, shopSlotUI.MarkUp);

        if (_shoppingCart.ContainsKey(data))
        {
            _shoppingCart[data]--;
            var newString = $"{data.itemName} ({price}$ ) x {_shoppingCart[data]}";
            _shoppingCartUI[data].SetItemText(newString);

            if (_shoppingCart[data] <= 0)
            {
                _shoppingCart.Remove(data);
                var tempObj = _shoppingCartUI[data].gameObject;
                _shoppingCartUI.Remove(data);
                Destroy(tempObj);
            }

        }

        _basketTotal -= price;
        _cartTotalText.text = $"Total: {_basketTotal}G";

        if (_basketTotal <= 0 && _cartTotalText.IsActive())
        {
            _cartTotalText.enabled = false;
            _buyButton.gameObject.SetActive(false);
            ClearItemPreview();
            return;

        }

        CheckCartVsPlayerGold();
    }

    private void ClearItemPreview()
    {
        _itemPreview.sprite = null;
        _itemPreview.color = Color.clear;
        _itemPreviewName.text = "";
        _itemPreviewDescription.text = "";

    }

    public void AddItemToCart(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;

        UpdateItemPreview(shopSlotUI);

        var price = GetModifiedPrice(data, 1, shopSlotUI.MarkUp);

        if (_shoppingCart.ContainsKey(data))
        {
            _shoppingCart[data]++;
            var newString = $"{data.itemName} ({price}$ ) x {_shoppingCart[data]}";
            _shoppingCartUI[data].SetItemText(newString);

        }
        else
        {
            _shoppingCart.Add(data, 1);

            var shoppingCartTextObj = Instantiate(_cartSlotPrefab, _shoppingListContent.transform);
            var newString = $"{data.itemName} ({price}$) x1";
            shoppingCartTextObj.SetItemText(newString);
            _shoppingCartUI.Add(data, shoppingCartTextObj);
        }
        _basketTotal += price;
        _cartTotalText.text = $"Total: {_basketTotal}G";

        if (_basketTotal > 0 && !_cartTotalText.IsActive())
        {
            _cartTotalText.enabled = true;
            _buyButton.gameObject.SetActive(true);
        }

        CheckCartVsPlayerGold();
    }


    public static int GetModifiedPrice(ItemInventoryData data, int amount, float MarkUp)
    {
        var baseValue = data.MoneyValue * amount;

        return Mathf.FloorToInt(baseValue + baseValue * MarkUp);
    }



    private void CheckCartVsPlayerGold()
    {
        var goldCheck = _isSelling ? _shopSystem.traderGold : _playerInventory.primaryInventorySystem.Gold;

        _cartTotalText.color = _basketTotal > goldCheck ? Color.red : Color.white;

        if (_isSelling || _playerInventory.primaryInventorySystem.CheckInventoryRemain(_shoppingCart)) return;

        _cartTotalText.text = "Not Enough Room in Inventory";
        _cartTotalText.color = Color.red;



    }

    public void UpdateItemPreview(ShopSlotUI shopSlotUi)
    {
        var data = shopSlotUi.AssignedItemSlot.ItemData;

        _itemPreview.sprite = data.icon;
        _itemPreview.color = Color.white;
        _itemPreviewName.text = data.itemName;
        _itemPreviewDescription.text = data.Description;
    }

    public void OnBuyTabPressed()
    {
       _isSelling = false;

        RefreshDisplay();
    }
    public void OnSellTabPressed()
    {
        _isSelling = true;

        RefreshDisplay();
    }

    
}
