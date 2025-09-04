using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class UIController : MonoBehaviour
{
    public DynamicInventoryDisplay InventoryPanel;
    public DynamicInventoryDisplay playerBackpackDisplay;

    private void Awake()
    {
        InventoryPanel.gameObject.SetActive(false);
        playerBackpackDisplay.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
        PlayerInventory.OnPlayerInventoryDisplayRequested += DisplayPlayerBackpack;

    }

    private void OnDisable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
        PlayerInventory.OnPlayerInventoryDisplayRequested += DisplayPlayerBackpack;

    }

    private void Update()
    {
        if (InventoryPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            InventoryPanel.gameObject.SetActive(false);

        }

        if (playerBackpackDisplay.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            playerBackpackDisplay.gameObject.SetActive(false);
        }
    }

    private void DisplayInventory(InventorySystem invToDisplay, int offset)
    {
        InventoryPanel.gameObject.SetActive(true);
        InventoryPanel.RefreshDynamicInventoryDisplay(invToDisplay, offset);
    }
    private void DisplayPlayerBackpack(InventorySystem invToDisplay, int offset)
    {
        playerBackpackDisplay.gameObject.SetActive(true);
        playerBackpackDisplay.RefreshDynamicInventoryDisplay(invToDisplay, offset);
    }




}