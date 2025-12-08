using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(IdSystem))]
public class GoldPickUp : MonoBehaviour, IInteractable
{
    public float PickUpRange = 2f; // Range within which the gold can be interacted with
    [SerializeField] private int goldAmount = 10; // Amount of gold in the pile

    [SerializeField] private float _rotationSpeed = 0f; // Speed of rotation

    private SphereCollider sphereCollider;
    public UnityAction<IInteractable> OnInteract { get; set; }

    private string id;

    private void Awake()
    {
        id = GetComponent<IdSystem>().Id;
        SaveLoad.OnLoadGame += LoadGoldData;

        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = PickUpRange;

    }

    private void Update()
    {
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
    }

    private void LoadGoldData(SaveData data)
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
        SaveLoad.OnLoadGame -= LoadGoldData;
    }

    public void Interact(Interactor interactor, out bool success)
    {
        success = false;

        var playerInventory = interactor.GetComponent<PlayerInventory>();
        if (playerInventory != null)
        {
            // Use the GainGold function from InventorySystem to increase the player's gold
            playerInventory.primaryInventorySystem.GainGold(goldAmount);
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
public struct GoldPickUpSaveData
{
    public int GoldAmount;
    public Vector3 Position;
    public Quaternion Rotation;

    public GoldPickUpSaveData(int _goldAmount, Vector3 _position, Quaternion _rotation)
    {
        GoldAmount = _goldAmount;
        Position = _position;
        Rotation = _rotation;
    }
}