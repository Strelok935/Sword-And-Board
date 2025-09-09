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

    [SerializeField] private float thrownForce; // Force applied when throwing the item
    public InventorySlot AssignedInventorySlot; // The inventory slot assigned to the mouse item


    private Transform _playerTransform;
    private Transform _dropPointTransform; // The transform representing the drop point
    public float _itemDropOffset = 1.5f;
    private void Awake() // Initialize the mouse item data
    {
        itemImage.color = Color.clear;
        itemImage.preserveAspect = true;
        itemCountText.text = string.Empty;
        _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        if (_playerTransform == null)
        {
            Debug.LogError("Player Transform not found. Ensure the player GameObject is tagged 'Player'.");
        }
        if (thrownForce <= 0)
        {
            thrownForce = 5f; // Default force if not set
            Debug.LogWarning("Thrown force not set. Using default value of 5.");
        }
        // Find the Drop Point transform
        _dropPointTransform = _playerTransform.Find("DropPoint");
        if (_dropPointTransform == null)
        {
            Debug.LogError("Drop Point not found. Ensure there is an empty GameObject named 'DropPoint' as a child of the Player.");
        }
            
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
        UpdateMouseSlot();

    }

    public void UpdateMouseSlot() // Update the mouse slot with the given inventory slot
    {
      
        itemImage.sprite = AssignedInventorySlot.ItemData.icon;
        itemCountText.text = AssignedInventorySlot.StackSize > 1 ? AssignedInventorySlot.StackSize.ToString() : string.Empty;
        itemImage.color = Color.white;
    } 

   private void Update()
    {
        if (AssignedInventorySlot != null && AssignedInventorySlot.ItemData != null)
        {
            // Update the position of the mouse item to follow the cursor
            transform.position = Mouse.current.position.ReadValue();

            // Check if the left mouse button is clicked and the pointer is not over a UI element
            if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject())
            {
                if (AssignedInventorySlot.ItemData.WorldModelPrefab != null)
                {
                    // Use the Drop Point position for instantiation
                    Vector3 dropPosition = _dropPointTransform != null
                        ? _dropPointTransform.position
                        : _playerTransform.position + _playerTransform.forward * _itemDropOffset;

                    GameObject droppedItem = Instantiate(
                        AssignedInventorySlot.ItemData.WorldModelPrefab,
                        dropPosition,
                        Quaternion.identity
                    );

                    // Apply a forward force to the Rigidbody to simulate throwing
                    Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 throwDirection = (_playerTransform.forward + Vector3.up * 0.5f + Random.insideUnitSphere * 0.1f).normalized; // Forward and slightly upward
                        rb.AddForce(throwDirection * thrownForce, ForceMode.Impulse);
                    }
                    if (AssignedInventorySlot.StackSize > 1)
                    {
                        AssignedInventorySlot.AddToStack(-1);
                        UpdateMouseSlot();
                    }
                    else
                    {
                        ClearSlot();
                    }
                }

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