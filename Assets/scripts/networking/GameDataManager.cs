using System;
using System.Collections.Generic;
using Constant;
using Cysharp.Threading.Tasks;
using Extensions;
using Unity.Services.CloudCode;
using Unity.Services.CloudSave;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    private Dictionary<string, object> gameData { get; } = new();
    
    private void Awake()
    {
        Instance = this;
    }
    
    //for collection, get as string and then deserialize using json
    public T GetGameData<T>(GameDataConstants.DataType type) => (T)gameData[type.ToString()];

    //for collection, set as serialized json string
    public void SetGameData<T>(GameDataConstants.DataType type, T value) => gameData[type.ToString()] = value;
    
    public async UniTask SetAndSaveGameData<T>(GameDataConstants.DataType type, T value)
    { 
        gameData[type.ToString()] = value;
        await SaveAllData();
    }
    
    public async UniTask SaveAllData()
    {
        await CloudCodeService.Instance.CallModuleEndpointAsync("Data", "GenerateUserID");
        //await CloudSaveService.Instance.Data.Player.SaveAsync(publicData, new SaveOptions(new PublicWriteAccessClassOptions()));
        //save using cloud code call
    }

    public async UniTask LoadAllData()
    {
        gameData.Clear();

        foreach (var (key, value) in await CloudSaveService.Instance.Data.Custom.LoadAsync("",GameDataConstants.Keys))
        {
            gameData[key] = this.DeserializeData(key, value.Value);
        }
    }
}
