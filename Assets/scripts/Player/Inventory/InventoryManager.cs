using System;
using System.Collections.Generic;
using System.Linq;
using Constant;
using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using NUnit.Framework.Internal;
using UnityEngine;

using Category = Constant.ItemConstants.ItemCategory;
using Access = Constant.PlayerDataConstants.DataAccessibility;
using ItemData = Models.ItemModel.ItemData;
using ProtectedData = Constant.PlayerDataConstants.ProtectedDataType;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    //crop/fish/building basedata(SO).ID = itemSO.ID
    public GameObject ItemSlotPrefab { get; private set; }
    [SerializeField] private ItemSO[] AllItemBaseData;

    public Dictionary<Category, List<ItemData>> AllItems { get; private set; } = new(3)
    {
        [Category.Crop] = new List<ItemData>(), 
        [Category.Fish] = new List<ItemData>(), 
        [Category.Building] = new List<ItemData>() 
    };

    private void Awake()
    {
        Instance = this;
        
        AllItemBaseData = AllItemBaseData.OrderBy(x => x.ID).ToArray();
        
        LoadData().Forget(); // need ~3 second buffer
    }
    
    public async UniTask UpdateItem(Category category, int baseDataID, int amount) //expose
    {
        List<ItemData> items = AllItems[category];
        
        ItemSO baseData = AllItemBaseData.First(x => x.ID == baseDataID);
        int index = items.FindIndex(x => x.ID == baseData.ID);

        if (index < 0)
        {
            items.Add(new(baseData, amount));
        }
        else
        {
            ItemData temp = items[index];
            temp.Amount += amount;
            items[index] = temp;
        }
        
        await SaveData();
    }

    public async UniTask UpdateItem(Category category, List<(int,int)> indexes) //expose
    {
        List<ItemData> items = AllItems[category];
        
        foreach (var (ID, amount) in indexes)
        {
            ItemSO baseData = AllItemBaseData.First(x => x.ID == ID);
            int index = items.FindIndex(x => x.ID == baseData.ID);
            
            if (index < 0)
            {
                items.Add(new(baseData, amount));
            }
            else
            {  
                ItemData temp = items[index];
                temp.Amount += amount;
                items[index] = temp;
            }
        }

        await SaveData();
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
            List<ItemData> items = AllItems[element.Category];
            
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