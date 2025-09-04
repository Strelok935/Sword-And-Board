using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.InputSystem;

public class MouseItemData : MonoBehaviour
{
    [SerializeField] private Image itemImage; // The image component representing the item
    [SerializeField] private TextMeshProUGUI itemCountText; // The text component displaying the item count
    public InventorySlot AssignedInventorySlot; // The inventory slot assigned to the mouse item


    private void Awake() // Initialize the mouse item data
    {
        itemImage.color = Color.clear;
        itemCountText.text = string.Empty;
    }

        public void UpdateMouseSlot(InventorySlot invSlot) // Update the mouse slot with the given inventory slot
    {
        if (invSlot == null || invSlot.ItemData == null)
        {
            Debug.LogWarning("The InventorySlot or its ItemData is null. Clearing the mouse slot.");
            ClearSlot(); // Clear the mouse slot if the input is invalid
            return;
        }

        if (AssignedInventorySlot == null)
        {
            AssignedInventorySlot = new InventorySlot(); // Initialize AssignedInventorySlot if it's null
        }

        AssignedInventorySlot.AssignItem(invSlot);
        itemImage.sprite = invSlot.ItemData.icon;
        itemCountText.text = invSlot.StackSize > 1 ? invSlot.StackSize.ToString() : string.Empty;
        itemImage.color = Color.white;
    }

    private void Update() // Update the position of the mouse item
    {
        if (AssignedInventorySlot != null && AssignedInventorySlot.itemData != null)
        {
            transform.position = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject())
            {
                ClearSlot();
            }
        }
    }


    public void ClearSlot() // Clear the mouse slot
    {
        if (AssignedInventorySlot != null)
        {
            Debug.Log($"Clearing mouse slot with item: {AssignedInventorySlot.ItemData?.itemName}");
            AssignedInventorySlot.ClearSlot();
        }
        else
        {
            Debug.LogWarning("Mouse slot is already null.");
        }

        itemImage.color = Color.clear;
        itemCountText.text = string.Empty;
        AssignedInventorySlot = null;
    }

    public bool IsPointerOverUIObject() // Check if the pointer is over a UI object
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}