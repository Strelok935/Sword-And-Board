using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public abstract class InventoryHolder : MonoBehaviour
{
    [SerializeField] private int inventorySize;
    [SerializeField] protected InventorySystem primaryInventorySystem;
    [SerializeField] protected int offset = 10;

    public int Offset => offset;

    public InventorySystem InventorySystem => primaryInventorySystem;

    public static UnityAction<InventorySystem, int> OnDynamicInventoryDisplayRequested;

    protected virtual void Awake()
    {
        SaveLoad.OnLoadGame += LoadInventory;
        primaryInventorySystem = new InventorySystem(inventorySize);
    }

    protected abstract void LoadInventory(SaveData data);

}

[System.Serializable]
public struct SaveDataInventory
{
    public InventorySystem InvSyst;
    public Vector3 Position;
    public Quaternion Rotation;

    public SaveDataInventory(InventorySystem _invSyst, Vector3 _position, Quaternion _rotation)
    {
        InvSyst = _invSyst;
        Position = _position;
        Rotation = _rotation;
    }

    public SaveDataInventory(InventorySystem _invSyst)
    {
        InvSyst = _invSyst;
        Position = Vector3.zero;
        Rotation = Quaternion.identity;
    }
}
