using System.Collections.Generic;
using Constant;
using UnityEngine;

using ItemData = Models.ItemModel.ItemData;

public class ItemCategory : MonoBehaviour // for inventory UI components
{
    [SerializeField] private ItemConstants.ItemCategory category;
    [SerializeField] private Transform spawnAnchor;
    
    private List<ItemData> data;

    public void Init()
    {
        data = InventoryManager.Instance.AllItems[category];

        foreach (ItemData element in data)
        {
            Instantiate(InventoryManager.Instance.ItemSlotPrefab, spawnAnchor).GetComponent<ItemSlot>().Init(element);
        }
    }
}
