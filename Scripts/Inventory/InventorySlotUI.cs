using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemSprite; // Reference to the item sprite image
    [SerializeField] private TextMeshProUGUI itemCountText; // Reference to the item count text
    [SerializeField] private InventorySlot assignedInventorySlot; // Reference to the assigned inventory slot

    [SerializeField] private float doubleClickThreshold = 0.25f;
    private Coroutine clickRoutine;

    public InventoryDisplay ParentDisplay { get; private set; } // Reference to the parent inventory display
    public InventorySlot AssignedInventorySlot => assignedInventorySlot; // Reference to the assigned inventory slot

    private void Awake() // Initialize the inventory slot UI
    {
        ClearSlot();

        itemSprite.preserveAspect = true;   

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

    public void OnPointerClick(PointerEventData eventData) // Handle UI slot click events
    {
        if (clickRoutine != null)
        {
            StopCoroutine(clickRoutine);
            clickRoutine = null;
            ParentDisplay?.SlotDoubleClicked(this);
            return;
        }

        clickRoutine = StartCoroutine(HandleSingleClick());
    }

    private IEnumerator HandleSingleClick()
    {
        yield return new WaitForSecondsRealtime(doubleClickThreshold);
        ParentDisplay?.SlotClicked(this);
        clickRoutine = null;
    }
}
