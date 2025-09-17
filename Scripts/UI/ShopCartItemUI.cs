using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ShopCartItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemText;

    public void SetItemText(string newString)
    {
        _itemText.text = newString;
    }
}
