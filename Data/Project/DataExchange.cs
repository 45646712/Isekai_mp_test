using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;

namespace HelloWorld;

public class DataExchange
{
    [CloudCodeFunction("SaveProtectedData")]
    public async Task SaveProtectedData(IExecutionContext context, IGameApiClient gameApiClient, string key, object value)
    {
        try
        {
            await gameApiClient.CloudSaveData.SetItemAsync(context, context.AccessToken, context.ProjectId, context.PlayerId, new SetItemBody(key, value));
        }
        catch (ApiException ex)
        {
            throw new Exception($"Failed to save data for playerId {context.PlayerId}. Error: {ex.Message}");
        }
    }

    [CloudCodeFunction("LoadData")]
    public async Task<object> LoadProtectedData(IExecutionContext context, IGameApiClient gameApiClient, string key)
    {
        try
        {
            var result = await gameApiClient.CloudSaveData.GetItemsAsync(context, context.AccessToken, context.ProjectId, context.PlayerId, new List<string> { key });
            
            return result.Data.Results.First().Value;
        }
        catch (ApiException ex)
        {
            throw new Exception($"Failed to get data for playerId {context.PlayerId}. Error: {ex.Message}");
        }
    }

    [CloudCodeFunction("GenerateUserID")]
    public async Task<Int64> GenerateUserID(IExecutionContext context, IGameApiClient gameApiClient)
    {
        try
        {
            Int64 userID;

            QueryIndexBody query = new QueryIndexBody(new List<FieldFilter>
            {
                new("UserID", Int64.MaxValue, FieldFilter.OpEnum.LE)
            }, new List<string>
            {
                "UserID"
            }, 0, 1);

            ApiResponse<QueryIndexResponse> result = await gameApiClient.CloudSaveData.QueryPublicPlayerDataAsync(context, context.AccessToken, context.ProjectId, query);

            if (result.Data.Results.Count == 0)
            {
                userID = 10000000;
            }
            else
            {
                userID = (Int64)result.Data.Results.First().Data.First().Value + 1;
            }

            await gameApiClient.CloudSaveData.SetPublicItemAsync(context, context.AccessToken, context.ProjectId, context.PlayerId, new SetItemBody("UserID", userID));

            return userID;
        }
        catch (ApiException ex)
        {
            throw new Exception($"Failed to save data for playerId {context.PlayerId}. Error: {ex.Message}");
        }
    }
}

public class ModuleConfig : ICloudCodeSetup
{
    public void Setup(ICloudCodeConfig config)
    {
        config.Dependencies.AddSingleton(GameApiClient.Create());
    }
}