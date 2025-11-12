using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Constant;
using Cysharp.Threading.Tasks;
using Extensions;
using Newtonsoft.Json;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Internal;
using Unity.Services.CloudSave.Models.Data.Player;
using UnityEngine;

using Access = Constant.PlayerDataConstants.DataAccessibility;
using PublicData = Constant.PlayerDataConstants.PublicDataType;
using ProtectedData = Constant.PlayerDataConstants.ProtectedDataType;
using PrivateData = Constant.PlayerDataConstants.PrivateDataType;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    public IPlayerDataService Player;

    private Dictionary<Access, Dictionary<string, object>> PlayerData { get; } = new(3)
    {
        [Access.Public] = new Dictionary<string, object>(),
        [Access.Protected] = new Dictionary<string, object>(),
        [Access.Private] = new Dictionary<string, object>()
    };
    
    private void Awake()
    {
        Instance = this;
    }
    
    //for collection, get as string and then deserialize using json
    public T1 GetData<T1, T2>(Access access, T2 type) where T2 : Enum => (T1)PlayerData[access][type.ToString()];
    public Dictionary<string, object> GetData(Access access) => PlayerData[access];
    
    //for collection, parse in or set as json string
    public void UpdateData<T1, T2>(Access access, T2 type, T1 value) where T2 : Enum => PlayerData[access][type.ToString()] = value;
    public void UpdateData(Access access, Dictionary<string, object> data) => PlayerData[access] = data;
    public async UniTask UpdateAndSaveData<T1, T2>(Access access, T2 type, T1 value) where T2 : Enum
    {
        PlayerData[access][type.ToString()] = value;
        
        Dictionary<string, object> data = new()
        {
            { type.ToString(), value }
        };

        await Player.SaveAsync(data, this.GetSaveOptions(access));
    }

    public async UniTask SaveAllData()
    {
        await Player.SaveAsync(PlayerData[Access.Public], this.GetSaveOptions(Access.Public));
        await Player.SaveAsync(PlayerData[Access.Protected]);
    }

    public async UniTask<T1> LoadData<T1,T2>(Access access, T2 type) where T2 : Enum
    {
        foreach (var (key, value) in await Player.LoadAsync(PlayerDataConstants.GetKey(type), this.GetLoadOptions(access)))
        {
            PlayerData[access][type.ToString()] = access switch
            {
                Access.Public => this.DeserializeData<PublicData>(key, value.Value),
                Access.Protected => this.DeserializeData<ProtectedData>(key, value.Value),
                Access.Private => this.DeserializeData<PrivateData>(key, value.Value),
                _ => null
            };
        }

        if (!PlayerData[access].TryGetValue(type.ToString(), out var data))
        {
            return default;
        }
        
        return (T1)data;
    }
    
    public async UniTask LoadAllData()
    {
        foreach (var (key, value) in PlayerData)
        {
            PlayerData[key].Clear();
        }

        foreach (var (key, value) in await Player.LoadAllAsync(this.GetLoadAllOptions(Access.Public)))
        {
            PlayerData[Access.Public][key] = this.DeserializeData<PublicData>(key, value.Value);
        }

        foreach (var (key, value) in await Player.LoadAllAsync(this.GetLoadAllOptions(Access.Protected)))
        {
            PlayerData[Access.Protected][key] = this.DeserializeData<ProtectedData>(key, value.Value);
        }

        foreach (var (key, value) in await Player.LoadAllAsync(this.GetLoadAllOptions(Access.Private)))
        {
            PlayerData[Access.Private][key] = this.DeserializeData<PrivateData>(key, value.Value);
        }
        
        await this.ValidateBasePlayerData();
        
        SessionManager.Instance.UpdateSessionHostInfo();
    }
}