using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class TopUIController : MonoBehaviour
{
    [SerializeField] private TraderScreen _traderScreen;


    private void Awake()
    {
        _traderScreen.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Trader.OnOpenTrader += DisplayTrader;

    }

    private void OnDisable()
    {
        Trader.OnOpenTrader -= DisplayTrader;
    }

    private void Update()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame) _traderScreen.gameObject.SetActive(false);
    }

    private void DisplayTrader(TradeSystem tradeSystem, PlayerInventory playerInventory)
    {
        _traderScreen.gameObject.SetActive(true);
        _traderScreen.DisplayTradeWindow(tradeSystem, playerInventory);
    }
 
}
