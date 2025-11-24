using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Extensions;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.CloudSave.Internal;
using UnityEngine;

using GameDataType = Unity.Services.CloudCode.GeneratedBindings.Data.DataConstants_GameDataType;
using Access = Unity.Services.CloudCode.GeneratedBindings.Data.DataConstants_DataAccessibility;
using DataOp = Unity.Services.CloudCode.GeneratedBindings.Data.DataConstants_DataOperations;
    
public class CloudCodeManager : MonoBehaviour
{
    public static CloudCodeManager Instance;
    
    public AccountEndpointsBindings AccountModule { get; private set; }
    public PlayerDataEndpointsBindings PlayerModule { get; private set; }
    public GameDataEndpointsBindings GameModule { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        AccountModule = new(CloudCodeService.Instance);
        PlayerModule = new(CloudCodeService.Instance);
        GameModule = new(CloudCodeService.Instance);
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
    
    public async UniTask ValidateAccountData() => await AccountModule.ValidateAccountData();
    
    //dev only functions
    public async UniTask SaveGameData(GameDataType type, string ID, string data) => await GameModule.SaveGameData(type, ID, data);
}
