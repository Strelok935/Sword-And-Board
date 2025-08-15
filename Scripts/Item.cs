// 7/29/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;

public class Item : MonoBehaviour
{
    public InventoryItemData itemData; // Reference to the ScriptableObject for this item
    public int quantity = 1; // Quantity of the item

    public void OnPickup()
    {
        // Add any additional logic here (e.g., play a sound or animation)
        Destroy(gameObject); // Remove the item from the scene
    }
}
