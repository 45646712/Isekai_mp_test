using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Cysharp.Threading.Tasks;
using Extensions;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using UnityEngine;

using ItemType = Unity.Services.CloudCode.GeneratedBindings.Data.ItemConstants_ItemType;
using GameDataType = Unity.Services.CloudCode.GeneratedBindings.Data.DataConstants_GameDataType;
using Access = Unity.Services.CloudCode.GeneratedBindings.Data.DataConstants_DataAccessibility;
using DataOp = Unity.Services.CloudCode.GeneratedBindings.Data.DataConstants_DataOperations;
    
public class CloudCodeManager : MonoBehaviour
{
    public static CloudCodeManager Instance;

    private AccountEndpointsBindings AccountModule;
    private PlayerDataEndpointsBindings PlayerModule;
    private GameDataEndpointsBindings GameModule;
    private CropEndpointsBindings CropModule;
    private ItemEndpointsBindings ItemModule;
    
    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        AccountModule = new(CloudCodeService.Instance);
        PlayerModule = new(CloudCodeService.Instance);
        GameModule = new(CloudCodeService.Instance);
        CropModule = new(CloudCodeService.Instance);
        ItemModule = new(CloudCodeService.Instance);
    }

    //remap functions to cloudcode (with serialization)
    public async UniTask SavePlayerData(Access access, string key, object value) => await PlayerModule.SavePlayerData(access, key, value.ToString());
    public async UniTask SaveMultiPlayerData(Access access, Dictionary<string, object> data) => await PlayerModule.SaveMultiPlayerData(access ,data.ToDictionary(x => x.Key, x => x.Value.ToString()));
    public async UniTask<object> LoadPlayerData(Access access, string key) => UniversalUtility.DeserializeData(key, await PlayerModule.LoadPlayerData(access, key));
    
    public async UniTask<Dictionary<string, object>> LoadMultiPlayerData(Access access, List<string> keys)
    {
        Dictionary<string, string> data = await PlayerModule.LoadMultiPlayerData(access, keys);
        
        return data.ToDictionary(x => x.Key, x => UniversalUtility.DeserializeData(x.Key, x.Value));
    }

    public async UniTask UpdatePlayerData(Access access, DataOp op, string key, int value) => await PlayerModule.UpdatePlayerData(access, op, key, value);
    public async UniTask UpdateMultiPlayerData(Access access, DataOp op, Dictionary<string, int> data) => await PlayerModule.UpdateMultiPlayerData(access, op, data);

    public async UniTask<string> LoadRawGameData(GameDataType type, int ID) => await GameModule.LoadGameData(type, ID);
    public async UniTask<List<string>> LoadMultiRawGameData(GameDataType type) => await GameModule.LoadMultiGameData(type);
    public async UniTask<T> LoadGameData<T>(GameDataType type, int ID) => JsonSerializer.Deserialize<T>(await GameModule.LoadGameData(type, ID));
    public async UniTask<List<T>> LoadMultiGameData<T>(GameDataType type)
    {
        List<string> result = await GameModule.LoadMultiGameData(type);
        
        return result.Select(x => JsonSerializer.Deserialize<T>(x)).ToList();
    }

    public async UniTask ValidateAccountData() => await AccountModule.ValidateAccountData();

    public async UniTask Plant(int slotID, int gameDataID) => await CropModule.Plant(slotID, gameDataID);
    public async UniTask MultiPlant(List<int> slotIDs, int gameDataID) => await CropModule.MultiPlant(slotIDs, gameDataID);
    public async UniTask Harvest(int slotID) => await CropModule.Harvest(slotID);
    public async UniTask MultiHarvest(List<int> slotIDs) => await CropModule.MultiHarvest(slotIDs);
    public async UniTask Remove(int slotID) => await CropModule.Remove(slotID);
    public async UniTask MultiRemove(List<int> slotIDs) => await CropModule.MultiRemove(slotIDs);
    public async UniTask<DateTimeOffset> TrackCropUpdate()
    {
        string result = await CropModule.TrackCropUpdate();
        return string.IsNullOrEmpty(result) ? default : JsonSerializer.Deserialize<DateTimeOffset>(result);
    }

    public async UniTask UpdateItem(DataOp op, ItemType type, int itemID, int amount) => await ItemModule.UpdateItem(op, type, itemID, amount);
    public async UniTask UpdateMultiItem(DataOp op, ItemType type, Dictionary<int, int> data) => await ItemModule.UpdateMultiItem(op, type, data.ToDictionary(x => x.ToString(), x => x.Value));
}