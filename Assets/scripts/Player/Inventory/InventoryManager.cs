using System;
using System.Collections.Generic;
using System.Linq;
using Constant;
using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using Unity.Services.CloudCode;
using UnityEngine;

using ItemType = Constant.ItemConstants.ItemType;
using Access = Constant.PlayerDataConstants.DataAccessibility;
using ItemData = Models.ItemModel.ItemData;
using ProtectedData = Constant.PlayerDataConstants.ProtectedDataType;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private GameObject ItemSlotPrefab;
    [SerializeField] private ItemSO[] AllItemBaseData;

    public Dictionary<ItemType, List<ItemData>> AllItems { get; private set; } = new(3)
    {
        [ItemType.Crop] = new List<ItemData>(), 
        [ItemType.Fish] = new List<ItemData>(), 
        [ItemType.Building] = new List<ItemData>() 
    };

    private void Awake()
    {
        Instance = this;
        
        AllItemBaseData = AllItemBaseData.OrderBy(x => x.ID).ToArray();
        
        LoadData().Forget(); // need ~3 second buffer
    }

    public async UniTask<bool> UpdateItem(ItemType type, int baseDataID, int amount, ItemConstants.ItemUpdateOperation ItemOp)
    {
        List<ItemData> items = AllItems[type];

        ItemSO baseData = AllItemBaseData.First(x => x.ID == baseDataID);
        int index = items.FindIndex(x => x.ID == baseData.ID);
        
        if (index < 0)
        {
            items.Add(new(baseData, 0));
            index = items.FindIndex(x => x.ID == baseData.ID);
        }
        
        ItemData instance = items[index];
        
        switch (ItemOp)
        {
            case ItemConstants.ItemUpdateOperation.Add:
                instance.Amount += amount;
                break;
            case ItemConstants.ItemUpdateOperation.Subtract:
                if (items[index].Amount < amount)
                {
                    Debug.LogError(LoggerConstants.InsufficientResource_Item);
                    return false;
                }

                instance.Amount -= amount;

                break;
        }
        
        items[index] = instance;
        
        await SaveData();
        
        return true;
    }

    public async UniTask<bool> UpdateItem(ItemType type, List<(int, int)> indexes, ItemConstants.ItemUpdateOperation ItemOp)
    {
        List<ItemData> items = AllItems[type];
        List<ItemData> instance = items;
        
        foreach (var (ID, amount) in indexes)
        {
            ItemSO baseData = AllItemBaseData.First(x => x.ID == ID);
            int index = instance.FindIndex(x => x.ID == baseData.ID);

            if (index < 0)
            {
                instance.Add(new(baseData, 0));
                index = instance.FindIndex(x => x.ID == baseData.ID);
            }
            
            ItemData element = instance[index];
            
            switch (ItemOp)
            {
                case ItemConstants.ItemUpdateOperation.Add:
                    element.Amount += amount;
                    instance[index] = element;
                    break;
                case ItemConstants.ItemUpdateOperation.Subtract:
                    if (instance[index].Amount < amount)
                    {
                        Debug.LogError(LoggerConstants.InsufficientResource_Item);
                        return false;
                    }

                    element.Amount -= amount;
                    instance[index] = element;

                    break;
            }
        }

        items = instance;
        
        await SaveData();

        return true;
    }

    private async UniTask SaveData()
    {
        string data = JsonConvert.SerializeObject(AllItems.SelectMany(x => x.Value.Select(y => new ItemModel.ItemUploadData(x.Key, y.ID, y.Amount))));
        await PlayerDataManager.Instance.UpdateAndSaveData(Access.Protected, ProtectedData.Inventory, data);
    }

    private async UniTask LoadData() //init function when opening inventory
    {
        string result = await PlayerDataManager.Instance.LoadData<string, ProtectedData>(Access.Protected, ProtectedData.Inventory);
        
        if (result == null)
        {
            return;
        }

        List<ItemModel.ItemUploadData> data = JsonConvert.DeserializeObject<List<ItemModel.ItemUploadData>>(result);

        foreach (ItemModel.ItemUploadData element in data)
        {
            List<ItemData> items = AllItems[element.ItemType];
            
            ItemSO baseData = AllItemBaseData.First(x=>x.ID == element.ID);
            int index = items.FindIndex(x => x.ID == baseData.ID);

            if (index < 0) // null -> default value
            {
                items.Add(new(baseData, element.Amount));
            }
            else
            {
                items[index] = new(baseData, element.Amount);
            }
        }
    }
}