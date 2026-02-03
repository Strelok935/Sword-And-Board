using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]

public class ItemInventoryData : ScriptableObject
{
    public int itemID = -1; // Unique identifier for the item;
    public string itemName; // Name of the item;
    public bool isEquippable; // Equipment Value;
    public bool isUniversal; // Universal Value;
    public Sprite icon; // Icon to display in the inventory;
    public bool isStackable; // Can this item stack?;
    public int maxStackSize = 64; // Maximum stack size (if stackable);
    [TextArea(4, 4)]
    public string Description; // Description of the item;

    public int MoneyValue; // Value of the item in game currency;

    public string ItemType; // Type of the item (e.g., Weapon, Armor, Consumable);
    public GameObject WorldModelPrefab; // Prefab to represent the item in the world;
    [Header("Flag On Pickup")]
    public bool setsFlagOnPickup;
    public string flagToSet;
    public bool flagValue = true;   // NEW

    [Header("Use Effects")]
    public bool isConsumable;
    public ItemUseEffect useEffect = new ItemUseEffect();

    public bool HasUsableEffect()
    {
        return useEffect != null && useEffect.HasAnyEffect();
    }
}

[Serializable]
public class ItemUseEffect
{
    [Header("Instant Recovery")]
    public float healAmount;
    public float staminaAmount;

    [Header("Timed Movement Modifiers")]
    public float durationSeconds;
    public float speedMultiplier = 1f;
    public float speedAdd;
    public float sprintSpeedMultiplier = 1f;
    public float sprintSpeedAdd;
    public float crouchSpeedMultiplier = 1f;
    public float crouchSpeedAdd;
    public float jumpHeightMultiplier = 1f;
    public float jumpHeightAdd;
    public float gravityMultiplier = 1f;
    public float gravityAdd;
    public float mouseSensitivityMultiplier = 1f;
    public float mouseSensitivityAdd;
    public float crouchTransitionSpeedMultiplier = 1f;
    public float crouchTransitionSpeedAdd;
    public float ladderClimbSpeedMultiplier = 1f;
    public float ladderClimbSpeedAdd;
    public float slideSpeedMultiplier = 1f;
    public float slideSpeedAdd;
    public float leanSpeedMultiplier = 1f;
    public float leanSpeedAdd;
    public float leanAngleMultiplier = 1f;
    public float leanAngleAdd;

    public bool HasAnyEffect()
    {
        return healAmount != 0f
            || staminaAmount != 0f
            || HasMovementChanges();
    }

    public bool HasMovementChanges()
    {
        return durationSeconds > 0f
            && (speedMultiplier != 1f || speedAdd != 0f
                || sprintSpeedMultiplier != 1f || sprintSpeedAdd != 0f
                || crouchSpeedMultiplier != 1f || crouchSpeedAdd != 0f
                || jumpHeightMultiplier != 1f || jumpHeightAdd != 0f
                || gravityMultiplier != 1f || gravityAdd != 0f
                || mouseSensitivityMultiplier != 1f || mouseSensitivityAdd != 0f
                || crouchTransitionSpeedMultiplier != 1f || crouchTransitionSpeedAdd != 0f
                || ladderClimbSpeedMultiplier != 1f || ladderClimbSpeedAdd != 0f
                || slideSpeedMultiplier != 1f || slideSpeedAdd != 0f
                || leanSpeedMultiplier != 1f || leanSpeedAdd != 0f
                || leanAngleMultiplier != 1f || leanAngleAdd != 0f);
    }

}
