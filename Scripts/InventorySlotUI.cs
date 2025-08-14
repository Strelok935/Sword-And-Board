using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private InventorySlot assignedInventorySlot;

    private Button button;

    public InventoryDisplay MainDisplay { get; private set; }
    public InventorySlot AssignedInventorySlot => assignedInventorySlot;

    private void Awake()
    {
        ClearSlot();

        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUiSlotClick);

        MainDisplay = transform.parent.GetComponentInParent<InventoryDisplay>();
    }

    public void Init(InventorySlot slot)
    {
        assignedInventorySlot = slot;
        UpdateUISlot(slot);
    }

    public void UpdateUISlot(InventorySlot slot)
    {

        if (slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.icon;
            itemSprite.color = Color.white;
            if (slot.StackSize > 1)
            {
                itemCountText.text = slot.StackSize.ToString();
            }
            else
            {
                itemCountText.text = string.Empty;
            }
        }
        else
        {
            ClearSlot();
        }
         
      
    }

    public void UpdateUISlot()
    {
        if (assignedInventorySlot != null)
        {
            UpdateUISlot(assignedInventorySlot);
        }
    }

    public void ClearSlot()
    {
        assignedInventorySlot?.ClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCountText.text = string.Empty;
    }

    private void OnUiSlotClick()
    {
        MainDisplay?.SlotClicked(this);
    }
}
