using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Constant;
using Cysharp.Threading.Tasks;
using Models;
using Unity.Services.CloudCode.GeneratedBindings.Data;
using static Constant.PlayerDataConstants;
using static Models.CropModel;

public class CropManager : MonoBehaviour
{
    public static CropManager Instance;
    
    [field: SerializeField] public Field[] Fields { get; private set; }

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
        
        LoadData().Forget(); // need ~3 second buffer
    }
    
    private async UniTask ValidateTimestamp(DateTimeOffset nextUpdateTime)
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
    
    public async UniTask Plant(int slotID, int gamedataID)
    {
        try
        {
            await CloudCodeManager.Instance.Plant(slotID, gamedataID);
            AllCrops[slotID].Init(await CloudCodeManager.Instance.LoadGameData<CropBaseData>(DataConstants_GameDataType.Crop, gamedataID), CropConstants.CropStatus.Growing);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async UniTask Plant(List<int> slotIDs ,  int gamedataID)
    {
        try
        {
            await CloudCodeManager.Instance.MultiPlant(slotIDs, gamedataID);

            CropBaseData baseData = await CloudCodeManager.Instance.LoadGameData<CropBaseData>(DataConstants_GameDataType.Crop, gamedataID);
            
            foreach (int element in slotIDs)
            {
                AllCrops[element].Init(baseData, CropConstants.CropStatus.Growing);
            }
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
            await CloudCodeManager.Instance.Harvest(slotID);
            AllCrops[slotID].Reset();
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
            await CloudCodeManager.Instance.MultiHarvest(slotIDs);
            
            foreach (int element in slotIDs)
            {
                AllCrops[element].Reset();
            }
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
                await CloudCodeManager.Instance.Remove(slotID);
                AllCrops[slotID].Reset();
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
            slotID = slotID.FindAll(x => AllCrops[x].data.Status != CropConstants.CropStatus.Matured);

            await CloudCodeManager.Instance.MultiRemove(slotID);
            
            foreach (int element in slotID)
            {
                AllCrops[element].Reset();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    private async UniTask ValidateTimer()
    {
        source.Cancel();
        source = new();
        
        //ValidateTimestamp(data.Where(x => x.MatureTime > DateTimeOffset.UtcNow).OrderBy(x => x.MatureTime).FirstOrDefault().MatureTime).Forget();
    }

    private async UniTask LoadData()
    {
        
        /*foreach (CropModel.CropUploadData element in data)
        {
            CropSO baseData = AllCropBaseData.First(x => x.ID == element.CropID);
            
            if (DateTimeOffset.UtcNow > element.MatureTime)
            {
                //AllCrops[element.SlotID].Init(baseData, element.MatureTime, CropConstants.CropStatus.Matured);
            }
            else
            {
                //AllCrops[element.SlotID].Init(baseData, element.MatureTime, CropConstants.CropStatus.Growing);
            }
        }*/

        await ValidateTimer();
    }
    
    private void OnDestroy() => StopAllCoroutines();
}