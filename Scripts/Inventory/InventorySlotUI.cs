using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image itemSprite; // Reference to the item sprite image
    [SerializeField] private TextMeshProUGUI itemCountText; // Reference to the item count text
    [SerializeField] private InventorySlot assignedInventorySlot; // Reference to the assigned inventory slot

    private Button button; // Reference to the button component

    public InventoryDisplay ParentDisplay { get; private set; } // Reference to the parent inventory display
    public InventorySlot AssignedInventorySlot => assignedInventorySlot; // Reference to the assigned inventory slot

    private void Awake() // Initialize the inventory slot UI
    {
        ClearSlot();

        itemSprite.preserveAspect = true;   

        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUiSlotClick);

        ParentDisplay = transform.parent.GetComponent<InventoryDisplay>();
    }

    public void Init(InventorySlot slot) // Initialize the inventory slot UI with the assigned inventory slot
    {
        assignedInventorySlot = slot;
        UpdateUISlot(slot);
    }

    public void UpdateUISlot(InventorySlot slot) // Update the UI elements to match the inventory slot
    {

        if (slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.icon;
            itemSprite.color = Color.white;
            if (slot.StackSize > 1) itemCountText.text = slot.StackSize.ToString();
            else itemCountText.text = string.Empty;

        }
        else
        {
            ClearSlot();
        }

        

    }

    public void UpdateUISlot() // Update the UI elements to match the assigned inventory slot
    {
        if (assignedInventorySlot != null)
        {
            UpdateUISlot(assignedInventorySlot);
        }
    }

    public void ClearSlot() // Clear the UI elements
    {
        assignedInventorySlot?.ClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCountText.text = string.Empty;
    }

    private void OnUiSlotClick() // Handle UI slot click events
    {
        ParentDisplay?.SlotClicked(this);
    }
}
