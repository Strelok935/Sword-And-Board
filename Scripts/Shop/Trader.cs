using System.Security.AccessControl;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(IdSystem))]
public class Trader : MonoBehaviour, IInteractable
{
    [SerializeField] private ShopItems _shopItems;
    [SerializeField] private TradeSystem _shopSystem;
    [Header("Dialogue")]
    [SerializeField] private DialogueData[] _dialogueOptions;

    public static UnityAction<TradeSystem, PlayerInventory> OnOpenTrader;

    private string _id;

    private ShopSaveData _shopSaveData;


    private void Awake()
    {
        _shopSystem =
        new TradeSystem
        (_shopItems.Items.Count, _shopItems.MaxAllowedGold, _shopItems.MarkupBuy, _shopItems.MarkupSell);

        foreach (var item in _shopItems.Items)
        {
            Debug.Log("Adding item to shop: " + item.ItemData.itemName + " Quantity: " + item.quantity);
            _shopSystem.AddToShop(item.ItemData, item.quantity);
        }

        _id = GetComponent<IdSystem>().Id;
        _shopSaveData = new ShopSaveData(_shopSystem);
    }


    private void Start()
    {
        if (!SaveGameManager.data.traderDictionary.ContainsKey(_id)) SaveGameManager.data.traderDictionary.Add(_id, _shopSaveData);
    }

    private void  OnEnable()
    {
        SaveLoad.OnLoadGame += LoadInventory;
    }

    private void LoadInventory(SaveData data)
    {
        if (data.traderDictionary.TryGetValue(_id, out ShopSaveData shopSaveData)) return;

        _shopSaveData = shopSaveData;
        _shopSystem = _shopSaveData.TradeSystem; 
    }

    private void OnDisable()
    {
        SaveLoad.OnLoadGame -= LoadInventory;
    }

    public UnityAction<IInteractable> OnInteract { get; set; } // Required by IInteractable
    public void Interact(Interactor interactor, out bool interactionSuccess)
    {
        if (DialogueManager.IsDialogueActive)
        {
            interactionSuccess = false;
            return;
        }

        var playerInventory = interactor.GetComponent<PlayerInventory>();

        if (playerInventory != null)
        {
            DialogueData chosenDialogue = GetValidDialogue();
            if (chosenDialogue != null && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(chosenDialogue);
                StartCoroutine(OpenTradeAfterDialogue(playerInventory));
            }
            else
            {
                OnOpenTrader?.Invoke(_shopSystem, playerInventory);
            }
            interactionSuccess = true;
        }
        else
        {
            interactionSuccess = false;
            Debug.Log("No Player Inventory found on Interactor");
        }
    }

    public void StopInteract()
    {
        // Optional: Add logic if needed when interaction stops
    }

    private DialogueData GetValidDialogue()
    {
        if (_dialogueOptions == null || _dialogueOptions.Length == 0) return null;

        foreach (var dialogue in _dialogueOptions)
        {
            if (dialogue == null) continue;

            if (dialogue.conditions == null || dialogue.conditions.Length == 0)
            {
                return dialogue;
            }

            bool valid = true;

            foreach (var condition in dialogue.conditions)
            {
                if (!condition.IsMet())
                {
                    valid = false;
                    break;
                }
            }

            if (valid) return dialogue;
        }

        return null;
    }

    private IEnumerator OpenTradeAfterDialogue(PlayerInventory playerInventory)
    {
        yield return new WaitUntil(() => !DialogueManager.IsDialogueActive);
        if (playerInventory != null)
        {
            OnOpenTrader?.Invoke(_shopSystem, playerInventory);
        }
    }

}

[System.Serializable]
public class ShopSaveData
{
    public TradeSystem TradeSystem;

    public ShopSaveData(TradeSystem tradeSystem)
    {
        TradeSystem = tradeSystem;
    }
}
