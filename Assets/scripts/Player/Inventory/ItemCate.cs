using System.Collections.Generic;
using Constant;
using UnityEngine;

using ItemData = Models.ItemModel.ItemData;

public class ItemCate : MonoBehaviour // for inventory UI components
{
    [SerializeField] private ItemConstants.ItemType type;
    [SerializeField] private Transform spawnAnchor;
    
    private List<ItemData> data;

    public void Init()
    {
        data = InventoryManager.Instance.AllItems[type];

        foreach (ItemData element in data)
        {
            //Instantiate(InventoryManager.Instance.ItemSlotPrefab, spawnAnchor).GetComponent<ItemSlot>().Init(element);
        }
    }
}
