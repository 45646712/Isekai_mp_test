using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Constant;
using Cysharp.Threading.Tasks;
using Extensions;
using Models;
using Newtonsoft.Json;

using Access = Constant.PlayerDataConstants.DataAccessibility;
using ProtectedData = Constant.PlayerDataConstants.ProtectedDataType;

public class CropManager : MonoBehaviour
{
    public static CropManager Instance;
    
    [field: SerializeField] public Field[] Fields { get; private set; }
    [field: SerializeField] public CropSO[] AllCropBaseData { get; private set; }

    public List<CropSlot> AllCrops { get; private set; } = new();
    
    private CancellationTokenSource source = new();
    
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
    
    private async UniTask ValidateTimestamp(DateTimeOffset nextUpdateTime) //TODO: modify 
    {
        if (nextUpdateTime > DateTimeOffset.Now)
        {
            try
            {
                await UniTask.Delay(nextUpdateTime - DateTimeOffset.Now, cancellationToken: source.Token);
                await LoadData();
            }
            catch (OperationCanceledException) { }
        }
    }
    
    public async UniTask Plant(int slotID, CropSO baseData)
    {
        try
        {
            if (AllCrops[slotID].isOccupied)
            {
                return;
            }
            
            if (baseData.Costs.ContainsKey(ItemConstants.ResourceType.Item))
            {
                if (!await InventoryManager.Instance.UpdateItem(ItemConstants.ItemType.Crop, baseData.ID, baseData.Costs[ItemConstants.ResourceType.Item], ItemConstants.ItemUpdateOperation.Subtract))
                {
                    return;
                }
            }

            if (!await PlayerDataManager.Instance.UpdatePlayerStatData(baseData.Costs, ItemConstants.ItemUpdateOperation.Subtract))
            {
                return;
            }
            
            AllCrops[slotID].Init(baseData, DateTimeOffset.UtcNow.AddSeconds(baseData.Costs[ItemConstants.ResourceType.Time]), CropConstants.CropStatus.Growing);

            await SaveData();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async UniTask Plant(List<int> slotIDs , CropSO baseData)
    {
        try
        {
            slotIDs = slotIDs.Where(x => !AllCrops[x].isOccupied).ToList();

            if (baseData.Costs.ContainsKey(ItemConstants.ResourceType.Item))
            {
                if (!await InventoryManager.Instance.UpdateItem(ItemConstants.ItemType.Crop, baseData.ID, baseData.Costs[ItemConstants.ResourceType.Item] * slotIDs.Count, ItemConstants.ItemUpdateOperation.Subtract))
                {
                    return;
                }
            }

            if (!await PlayerDataManager.Instance.UpdatePlayerStatData(baseData.Costs.ToDictionary(x=>x.Key,x=>x.Value * slotIDs.Count), ItemConstants.ItemUpdateOperation.Subtract))
            {
                return;
            }
            
            foreach (int element in slotIDs)
            {
                AllCrops[element].Init(baseData, DateTimeOffset.UtcNow.AddSeconds(baseData.Costs[ItemConstants.ResourceType.Time]), CropConstants.CropStatus.Growing);
            }
            
            await SaveData();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async UniTask Harvest(int slotID)
    {
        try
        {
            CropModel.CropData data = AllCrops[slotID].data;
            
            if (data.Status != CropConstants.CropStatus.Matured)
            {
                return;
            }

            await PlayerDataManager.Instance.UpdatePlayerStatData(data.Rewards, ItemConstants.ItemUpdateOperation.Add);
            await InventoryManager.Instance.UpdateItem(ItemConstants.ItemType.Crop, data.ID, data.Rewards[ItemConstants.ResourceType.Item], ItemConstants.ItemUpdateOperation.Add);
            
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
            slotIDs = slotIDs.Where(x => AllCrops[x].data.Status == CropConstants.CropStatus.Matured).ToList();
            
            if (slotIDs.Count == 0)
            {
                return;
            }

            List<CropModel.CropData> validSlotData = slotIDs.Select(x => AllCrops[x].data).ToList();
            
            await InventoryManager.Instance.UpdateItem(ItemConstants.ItemType.Crop, validSlotData.Select(x => (x.ID, x.Rewards[ItemConstants.ResourceType.Item])).ToList(), ItemConstants.ItemUpdateOperation.Add);
            //await PlayerDataManager.Instance.UpdatePlayerStatData(validSlotData.Select(x => x.Rewards).ToList(), ItemConstants.ItemUpdateOperation.Add);

            foreach (int element in slotIDs)
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

    public async UniTask Remove(int slotID)
    {
        try
        {
            if (AllCrops[slotID].data.Status != CropConstants.CropStatus.Matured)
            {
                AllCrops[slotID].Reset();
                await SaveData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    public async UniTask Remove(List<int> slotID)
    {
        try
        {
            foreach (int element in slotID.Where(x => AllCrops[x].data.Status != CropConstants.CropStatus.Matured))
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

        RestartValidation();
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
                AllCrops[element.SlotID].Init(baseData, element.MatureTime, CropConstants.CropStatus.Growing);
            }
        }

        RestartValidation();
        ValidateTimestamp(data.Where(x => x.MatureTime > DateTimeOffset.UtcNow).OrderBy(x => x.MatureTime).FirstOrDefault().MatureTime).Forget();
    }

    private void RestartValidation()
    {
        source.Cancel();
        source = new();
    }
    
    private void OnDestroy() => StopAllCoroutines();
}