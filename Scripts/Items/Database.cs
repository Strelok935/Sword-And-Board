using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System;


[CreateAssetMenu(fileName = "Database", menuName = "InventorySystem / Item Database", order = 1)]

public class Database : ScriptableObject
{
    [SerializeField] private List<ItemInventoryData> _itemDatabase;

    [ContextMenu("Set Item IDs")]
    public void SetItemIDs()
    {
        _itemDatabase = new List<ItemInventoryData>();

        var foundItems = Resources.LoadAll<ItemInventoryData>("ItemData").OrderBy(i => i.itemID).ToList();

        var HasIDInRange = foundItems.Where(i => i.itemID != -1 && i.itemID < foundItems.Count).OrderBy(i => i.itemID).ToList();
        var HasIDNotInRange = foundItems.Where(i => i.itemID != -1 && i.itemID >= foundItems.Count).OrderBy(i => i.itemID).ToList();
        var HasNoID = foundItems.Where(i => i.itemID == -1).OrderBy(i => i.itemID).ToList();
        var index = 0;

        for (int i = 0; i < foundItems.Count; i++)
        {
            ItemInventoryData itemToAdd;

            itemToAdd = HasIDInRange.Find(it => it.itemID == i);

            if (itemToAdd != null)
            {
                _itemDatabase.Add(itemToAdd);

            }
            else if (index < HasNoID.Count)
            {
                HasNoID[index].itemID = i;
                itemToAdd = HasNoID[index];
                index++;
                _itemDatabase.Add(itemToAdd);
            }
        }

        foreach (var item in HasIDNotInRange)
        {
            _itemDatabase.Add(item);
        }
    }
    
    public ItemInventoryData GetItemByID(int id)
    {
        return _itemDatabase.Find(i => i.itemID == id);
    }   
}
 