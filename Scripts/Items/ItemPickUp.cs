using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(IdSystem))]
public class ItemPickUp : MonoBehaviour, IInteractable
{
    public float PickUpRange = 2f; // Range within which the item can be interacted with
    public ItemInventoryData itemData;

    [SerializeField] private float _rotationSpeed = 0f; // Speed of rotation

    private SphereCollider sphereCollider;
    public UnityAction<IInteractable> OnInteract { get; set; } // Correctly implemented property

    private string id;

    private void Awake()
    {
        id = GetComponent<IdSystem>().Id;
        SaveLoad.OnLoadGame += LoadItemData;

        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = PickUpRange;

        SaveGameManager.data.activeItems.Add(id, new ItemPickUpSaveData(itemData, transform.position, transform.rotation));
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
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

    public void Interact(Interactor interactor, out bool success)
    {
        success = false;

        var inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory != null && inventory.AddToInventory(itemData, 1))
        {
            SaveGameManager.data.collectedItems.Add(id);
            success = true;
            Destroy(gameObject);
        }
    }

    public void StopInteract()
    {
        // No specific behavior needed for stopping interaction in this case
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
