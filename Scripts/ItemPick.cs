// 7/2/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

// 7/2/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine; // Add this to resolve MonoBehaviour and Sprite references
using TMPro;
public class ItemPick : MonoBehaviour
{
    public string ItemName; // Name of the item
    public Sprite ItemIcon; // Icon of the item
    public int Quantity = 1; // Quantity of the item
    public int MaxStackSize = 10; // Maximum stack size for the item



public void PickUp()
{
    Debug.Log($"Picked up item: {ItemName}");
    Destroy(gameObject);
}
}
