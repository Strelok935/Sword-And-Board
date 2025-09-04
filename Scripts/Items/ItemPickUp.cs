using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(IdSystem))]
public class ItemPickUp : MonoBehaviour
{
    public float PickUpRange = 2f; // Range within which the item can be picked up
    public ItemInventoryData itemData;

    private SphereCollider sphereCollider;

    [SerializeField] private ItemPickUpSaveData itemSaveData;
    private string id;

    private void Awake()
    {
        id = GetComponent<IdSystem>().Id;
        SaveLoad.OnLoadGame += LoadItemData;
        itemSaveData = new ItemPickUpSaveData(itemData, transform.position, transform.rotation);

        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = PickUpRange;
    }


    void Start()
    {
        SaveGameManager.data.activeItems.Add(id, itemSaveData);
    }

    private void LoadItemData(SaveData data)
    {
        if (data.collectedItems.Contains(id))
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (SaveGameManager.data.activeItems.ContainsKey(id))
        {
            SaveGameManager.data.activeItems.Remove(id);
        }
        SaveLoad.OnLoadGame -= LoadItemData;
    }

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.transform.GetComponent<PlayerInventory>();
        if (!inventory) return;

        if (inventory.AddToInventory(itemData, 1))
        {
            SaveGameManager.data.collectedItems.Add(id);
            // Optionally, you can destroy the item after picking it up
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
public struct ItemPickUpSaveData
{
    public ItemInventoryData ItemData;
    public Vector3 Position;
    public Quaternion Rotation;

    public ItemPickUpSaveData(ItemInventoryData _itemData, Vector3 _position, Quaternion _rotation)
    {
        ItemData = _itemData;
        Position = _position;
        Rotation = _rotation;
    }
}
