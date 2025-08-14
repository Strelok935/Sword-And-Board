using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

[RequireComponent(typeof(SphereCollider))]
public class ItemPick : MonoBehaviour
{
    public float PickUpRange = 2f; // Range within which the item can be picked up

    public ItemData ItemData; // Reference to the item data
    private SphereCollider sphereCollider;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = PickUpRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponent<InventoryHolder>();
        if (!inventory) return;

        if (inventory.InventorySystem.AddToInventory(ItemData, 1))
        {
            Destroy(this.gameObject);
        }
    }

}
