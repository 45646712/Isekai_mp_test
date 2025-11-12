using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;

namespace HelloWorld;

public class GameData
{
    [CloudCodeFunction("SaveGameData")]
    public async Task SaveGameData(IExecutionContext context, IGameApiClient gameApiClient, string key, object value)
    {
        try
        {
            await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.AccessToken, context.ProjectId, context.PlayerId, new SetItemBody(key, value));
        }
        catch (ApiException ex)
        {
            throw new Exception($"Failed to save data for playerId {context.PlayerId}. Error: {ex.Message}");
        }
    }
    
    /*[CloudCodeFunction("LoadGameData")]
    public async Task<object> LoadGameData(IExecutionContext context, IGameApiClient gameApiClient, string key)
    {
        try
        {
            //var result = await gameApiClient.CloudSaveData.GetItemsAsync(context, context.AccessToken, context.ProjectId, context.PlayerId, new List<string> { key });
        }
        catch (ApiException ex)
        {
            throw new Exception($"Failed to get data for playerId {context.PlayerId}. Error: {ex.Message}");
        }
    }*/
}