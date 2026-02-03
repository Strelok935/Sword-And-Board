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

    private PlayerMovement player;
    private bool uiOpen = false;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }


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
        PlayerInventory.OnPlayerInventoryDisplayRequested -= DisplayPlayerBackpack;

    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            bool closedSomething = false;

            if (InventoryPanel.gameObject.activeInHierarchy)
            {
                InventoryPanel.gameObject.SetActive(false);
                closedSomething = true;
            }

            if (playerBackpackDisplay.gameObject.activeInHierarchy)
            {
                playerBackpackDisplay.gameObject.SetActive(false);
                closedSomething = true;
            }

            if (closedSomething)
                ExitUIMode();
        }
    }

    private void DisplayInventory(InventorySystem invToDisplay, int offset)
    {
        InventoryPanel.gameObject.SetActive(true);
        InventoryPanel.RefreshDynamicInventoryDisplay(invToDisplay, offset);
        EnterUIMode();
    }

    private void DisplayPlayerBackpack(InventorySystem invToDisplay, int offset)
    {
        playerBackpackDisplay.gameObject.SetActive(true);
        playerBackpackDisplay.RefreshDynamicInventoryDisplay(invToDisplay, offset);
        EnterUIMode();
    }

    private void EnterUIMode()
    {
        if (uiOpen) return;
        uiOpen = true;

        player.SetControlsLocked(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ExitUIMode()
    {
        if (!uiOpen) return;
        uiOpen = false;

        player.SetControlsLocked(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }




}