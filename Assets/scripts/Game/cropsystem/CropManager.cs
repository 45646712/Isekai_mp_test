using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.CloudCode.GeneratedBindings.Data;

using static Constant.DataConstants;
using static Constant.CropConstants;
using static Models.CropModel;

public class CropManager : NetworkBehaviour
{
    public static CropManager Instance;
    
    [field: SerializeField] public Field[] Fields { get; private set; }

    public List<CropSlot> AllCrops { get; private set; } = new();
    
    private CancellationTokenSource source = new();

    private bool isLoadComplete;
    
    private void Awake()
    {
        Instance = this;
    }

    private async UniTaskVoid Start()
    {
        NetworkManager.Singleton.OnConnectionEvent += (manager, data) => UniTask.Action(async () =>
        {
            if (data.EventType == ConnectionEvent.ClientConnected && NetworkManager.IsHost)
            {
                await InitLock();
            }
        }).Invoke();
        
        AllCrops = Fields.SelectMany(x => x.Crops).ToList();

        await InitLock();
        await LoadData();
    }

    public override void OnNetworkSpawn() //for clients
    {
        if (!NetworkManager.IsHost)
        {
            Destroy(gameObject);
        }
    }

    private async UniTask TrackNextUpdate(DateTimeOffset nextUpdateTime)
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
        else if (nextUpdateTime <= DateTimeOffset.Now && !isLoadComplete)
        {
            await LoadData();
            isLoadComplete = true;
        }
    }

    public async UniTask Plant(int slotID, int gamedataID)
    {
        try
        {
            await CloudCodeManager.Instance.Plant(slotID, gamedataID);
            string rawBaseData = await CloudCodeManager.Instance.LoadRawGameData(DataConstants_GameDataType.Crop, gamedataID);

            AllCrops[slotID].InitRpc(rawBaseData, status: CropStatus.Growing);

            isLoadComplete = false;
            await TrackCropUpdate();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async UniTask Plant(List<int> slotIDs, int gamedataID)
    {
        try
        {
            await CloudCodeManager.Instance.MultiPlant(slotIDs, gamedataID);
            string rawBaseData = await CloudCodeManager.Instance.LoadRawGameData(DataConstants_GameDataType.Crop, gamedataID);

            foreach (int element in slotIDs)
            {
                AllCrops[element].InitRpc(rawBaseData, status: CropStatus.Growing);
            }

            isLoadComplete = false;
            await TrackCropUpdate();
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
            AllCrops[slotID].ResetRpc();
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
                AllCrops[element].ResetRpc();
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
            if (AllCrops[slotID].data.Status != CropStatus.Matured)
            {
                await CloudCodeManager.Instance.Remove(slotID);
                AllCrops[slotID].ResetRpc();
                
                isLoadComplete = false;
                await TrackCropUpdate();
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
            slotID = slotID.FindAll(x => AllCrops[x].data.Status != CropStatus.Matured);

            await CloudCodeManager.Instance.MultiRemove(slotID);
            
            foreach (int element in slotID)
            {
                AllCrops[element].ResetRpc();
            }
            
            isLoadComplete = false;
            await TrackCropUpdate();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    private async UniTask TrackCropUpdate()
    {
        source.Cancel();
        source = new();

        TrackNextUpdate(await CloudCodeManager.Instance.TrackCropUpdate()).Forget();
    }

    private async UniTask InitLock()
    {
        int validSlots = (int)await CloudCodeManager.Instance.LoadPlayerData(DataConstants_DataAccessibility.Protected, nameof(ProtectedDataType.UnlockedCropSlots));
        
        for (int i = 0; i < AllCrops.Count; i++)
        {
            AllCrops[i].slotID = i;
            
            if (i >= validSlots)
            {
                AllCrops[i].LockRpc();
            }
        }

        foreach (Field element in Fields.Where(x => x.Crops.First().slotID <= validSlots))
        {
            element.Init();
        }
    }
    
    private async UniTask LoadData()
    {
        List<string> baseData = await CloudCodeManager.Instance.LoadMultiRawGameData(DataConstants_GameDataType.Crop);
        Dictionary<int, string> slotData = (Dictionary<int, string>)await CloudCodeManager.Instance.LoadPlayerData(DataConstants_DataAccessibility.Protected, nameof(ProtectedDataType.CropData));

        if (slotData is null or { Count: 0 })
        {
            await TrackCropUpdate();
            return;
        }
        
        Dictionary<int, CropUploadData> data = slotData.ToDictionary(x => x.Key, x => JsonSerializer.Deserialize<CropUploadData>(x.Value));
        
        foreach (var (slotID, value) in data)
        {
            AllCrops[slotID].InitRpc(baseData[value.CropID - 1], value.MatureTime.ToString(), DateTimeOffset.UtcNow > value.MatureTime ? CropStatus.Matured : CropStatus.Growing);
        }

        await TrackCropUpdate();
    }
}