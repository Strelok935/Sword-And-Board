using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] private Image _itemSprite;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemCount;
    [SerializeField] private ShopSlot _assignedItemSlot;

    public ShopSlot AssignedItemSlot => _assignedItemSlot;

    [SerializeField] private Button _addItemToCart;
    [SerializeField] private Button _removeItemFromCart;

    private int _tempAmount;

    public TraderScreen ParentDisplay { get; private set; }
    public float MarkUp { get; private set; }

    private void Awake()
    {
        _itemSprite.sprite = null;
        _itemSprite.preserveAspect = true;
        _itemSprite.color = Color.clear;
        _itemName.text = "";
        _itemCount.text = "";

        _addItemToCart?.onClick.AddListener(AddItemToCart);
        _removeItemFromCart?.onClick.AddListener(RemoveItemFromCart);
        ParentDisplay = transform.parent.GetComponentInParent<TraderScreen>();

    }
    
    public void Init(ShopSlot slot, float markUp)
    {
        _assignedItemSlot = slot;
        MarkUp = markUp;
        _tempAmount = slot.StackSize;
        UpdateUiSlot();
    }

    private void UpdateUiSlot()
    {
        if (_assignedItemSlot.ItemData != null)
        {
            _itemSprite.sprite = _assignedItemSlot.ItemData.icon;
            _itemSprite.color = Color.white;
            _itemCount.text = _assignedItemSlot.StackSize.ToString();
            var modifiedPrice = TraderScreen.GetModifiedPrice(_assignedItemSlot.ItemData, 1, MarkUp);
            _itemName.text = $"{_assignedItemSlot.ItemData.itemName} - {modifiedPrice}G";

        }
        else
        {
            _itemSprite.sprite = null;
            _itemSprite.preserveAspect = true;
            _itemSprite.color = Color.clear;
            _itemName.text = "";
            _itemCount.text = "";
        }
    } 
    
    public void RemoveItemFromCart()
    {
       if (_tempAmount == _assignedItemSlot.StackSize) return;
        
            _tempAmount++;
            ParentDisplay.RemoveItemFromCart(this);
            _itemCount.text = _tempAmount.ToString();
    }
    public void AddItemToCart()
    {
        if (_tempAmount <= 0) return;

        _tempAmount--;
        ParentDisplay.AddItemToCart(this);
        _itemCount.text = _tempAmount.ToString();

    }


}
