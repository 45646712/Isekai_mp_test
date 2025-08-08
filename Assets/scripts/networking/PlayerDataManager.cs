using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Constant;
using Cysharp.Threading.Tasks;
using Extensions;
using Model;
using Newtonsoft.Json;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Internal.Http;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.VisualScripting;
using UnityEngine;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    private Dictionary<string, object> publicData { get; } = new();
    private Dictionary<string, object> protectedData { get; } = new();
    private Dictionary<string, object> privateData { get; } = new();

    private void Awake()
    {
        Instance = this;
    }
    
    //for collection, get as string and then deserialize using json
    public T GetPublicData<T>(PlayerDataConstant.PublicDataType type) => (T)publicData[type.ToString()];
    public T GetProtectedData<T>(PlayerDataConstant.PublicDataType type) => (T)protectedData[type.ToString()];
    public T GetPrivateData<T>(PlayerDataConstant.PublicDataType type) => (T)privateData[type.ToString()];
    
    //for collection, set as serialized json string
    public void SetPublicData<T>(PlayerDataConstant.PublicDataType type, T value) => publicData[type.ToString()] = value;
    public void SetProtectedData<T>(PlayerDataConstant.PublicDataType type, T value) => protectedData[type.ToString()] = value;
    
    public async UniTask SetAndSavePublicData<T>(PlayerDataConstant.PublicDataType type, T value)
    { 
        publicData[type.ToString()] = value;
        await CloudSaveService.Instance.Data.Player.SaveAsync(publicData, new SaveOptions(new PublicWriteAccessClassOptions()));
    }
    
    public async UniTask SetAndSaveProtectedData<T>(PlayerDataConstant.PublicDataType type, T value)
    { 
        protectedData[type.ToString()] = value;
        await CloudSaveService.Instance.Data.Player.SaveAsync(protectedData);
    }
    
    public async UniTask SaveAllData()
    {
        await CloudSaveService.Instance.Data.Player.SaveAsync(publicData, new SaveOptions(new PublicWriteAccessClassOptions()));
        await CloudSaveService.Instance.Data.Player.SaveAsync(protectedData);
    }

    public async UniTask LoadAllData()
    {
        publicData.Clear();
        protectedData.Clear();
        privateData.Clear();

        foreach (var (key, value) in await CloudSaveService.Instance.Data.Player.LoadAsync(PlayerDataConstant.PublicKeys, new LoadOptions(new PublicReadAccessClassOptions())))
        {
            publicData[key] = this.DeserializeData(key,value.Value);
        }
        foreach (var (key, value) in await CloudSaveService.Instance.Data.Player.LoadAsync(PlayerDataConstant.ProtectedKeys))
        {
            protectedData[key] = this.DeserializeData(key,value.Value);
        }
        foreach (var (key, value) in await CloudSaveService.Instance.Data.Player.LoadAsync(PlayerDataConstant.PrivateKeys, new LoadOptions(new ProtectedReadAccessClassOptions())))
        {
            privateData[key] = this.DeserializeData(key,value.Value);
        }

        if (!publicData.TryGetValue(PlayerDataConstant.PublicDataType.UserID.ToString(), out object userid))
        {
            Debug.LogWarning("userID not exist, generating new userID");
            await this.GenerateUserID();
        }

        SessionManager.Instance.UpdateSessionHostInfo();
    }
}
