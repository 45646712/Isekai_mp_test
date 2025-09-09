using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Constant;
using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

using Access = Constant.PlayerDataConstants.DataAccessibility;
using ProtectedData = Constant.PlayerDataConstants.ProtectedDataType;

public class CropManager : MonoBehaviour
{
    public static CropManager Instance;
    
    [field: SerializeField] public Field[] Fields { get; private set; }
    [SerializeField] private CropSO[] AllCropBaseData;

    public List<CropSlot> AllCrops { get; private set; } = new();

    private void Awake()
    {
        Instance = this;

        if (!SessionManager.Instance.CurrentSession.IsHost)
        {
            enabled = false;
            return;
        }

        AllCrops = Fields.SelectMany(x => x.Crops).ToList();
        AllCropBaseData = AllCropBaseData.OrderBy(x => x.ID).ToArray();
        
        LoadData().Forget(); // need ~3 second buffer
    }

    private async UniTask ValidateTimestamp(DateTimeOffset nextUpdateTime)
    {
        while (this != null)
        {
            if (nextUpdateTime == default)
            {
                await LoadData(); //single check
                break;
            }
            
            if (DateTimeOffset.UtcNow > nextUpdateTime)
            {
                await LoadData();
                break;
            }

            await UniTask.Yield();
        }
    }

    public async UniTask Plant(int slotID, int baseDataID)
    {
        if (AllCrops[slotID].isOccupied)
        {
            return;
        }

        CropSO baseData = AllCropBaseData.First(x => x.ID == baseDataID);
        AllCrops[slotID].Init(baseData, DateTimeOffset.UtcNow.AddSeconds(baseData.MatureTime));

        await SaveData();
    }
    
    public async UniTask Plant(List<int> slotIDs , int baseDataID)
    {
        //extract SO value
        foreach (int element in slotIDs)
        {
            if (AllCrops[element].isOccupied)
            {
                continue;
            }

            CropSO baseData = AllCropBaseData.First(x => x.ID == baseDataID);
            AllCrops[element].Init(baseData, DateTimeOffset.UtcNow.AddSeconds(baseData.MatureTime));
        }
        
        await SaveData();
    }

    public async UniTask Harvest(int slotID)
    {
        try
        {
            if (slotID < 0)
            {
                return;
            }
            
            await InventoryManager.Instance.UpdateItem(ItemConstants.ItemCategory.Crop, AllCrops[slotID].data.ID, AllCrops[slotID].data.HarvestCount);
            AllCrops[slotID].Reset();
            await SaveData();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    public async UniTask Harvest(List<int> slotIDs)
    {
        try
        {
            await InventoryManager.Instance.UpdateItem(ItemConstants.ItemCategory.Crop, slotIDs.Select(x => (AllCrops[x].data.ID, AllCrops[x].data.HarvestCount)).ToList());
            
            foreach (var element in slotIDs.Where(element => element >= 0))
            {
                AllCrops[element].Reset();
            }
            
            await SaveData();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    private async UniTask SaveData()
    {
        List<CropModel.CropUploadData> data = AllCrops.Where(x => x.data.MatureTime != default).Select(x => new CropModel.CropUploadData(AllCrops.IndexOf(x), x.data.ID, x.data.MatureTime)).ToList();
        
        await PlayerDataManager.Instance.UpdateAndSaveData(Access.Protected, ProtectedData.CropData, JsonConvert.SerializeObject(data));
        
        ValidateTimestamp(data.Where(x => x.MatureTime > DateTimeOffset.UtcNow).OrderBy(x => x.MatureTime).FirstOrDefault().MatureTime).Forget();
    }

    private async UniTask LoadData()
    {
        string result = await PlayerDataManager.Instance.LoadData<string, ProtectedData>(Access.Protected, ProtectedData.CropData);

        if (result == null)
        {
            return;
        }

        List<CropModel.CropUploadData> data = JsonConvert.DeserializeObject<List<CropModel.CropUploadData>>(result);

        if (data.Count == 0)
        {
            return;
        }

        foreach (CropModel.CropUploadData element in data)
        {
            CropSO baseData = AllCropBaseData.First(x => x.ID == element.CropID);
            
            if (DateTimeOffset.UtcNow > element.MatureTime)
            {
                AllCrops[element.SlotID].Init(baseData, element.MatureTime, CropConstants.CropStatus.Matured);
            }
            else
            {
                AllCrops[element.SlotID].Init(baseData, element.MatureTime);
            }
        }
        
        ValidateTimestamp(data.Where(x => x.MatureTime > DateTimeOffset.UtcNow).OrderBy(x => x.MatureTime).FirstOrDefault().MatureTime).Forget();
    }

    private void OnDestroy() => StopAllCoroutines();
}