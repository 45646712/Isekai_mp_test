using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;

using static Data.DataConstants;

namespace Data;

public static class PlayerData
{
    public static async Task SavePlayerData(IExecutionContext context, IGameApiClient gameApiClient, DataAccessibility access, string key, object value)
    {
        switch (access)
        {
            case DataAccessibility.Public:
                await gameApiClient.CloudSaveData.SetPublicItemAsync(context, context.AccessToken, context.ProjectId, context.PlayerId!, new SetItemBody(key, value));
                return;
            case DataAccessibility.Protected:
                await gameApiClient.CloudSaveData.SetItemAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId!, new SetItemBody(key, value));
                return;
            case DataAccessibility.Private:
                await gameApiClient.CloudSaveData.SetProtectedItemAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId!, new SetItemBody(key, value));
                return;
            default:
                throw new InvalidOperationException();
        }
    }
    
    public static async Task SaveMultiPlayerData(IExecutionContext context, IGameApiClient gameApiClient, DataAccessibility access, Dictionary<string, object> data)
    {
        SetItemBatchBody extractedData = new SetItemBatchBody(data.Select(x => new SetItemBody(x.Key, x.Value)).ToList());

        if (extractedData.Data.Count == 0)
        {
            return;
        }
        
        switch (access)
        {
            case DataAccessibility.Public:
                await gameApiClient.CloudSaveData.SetPublicItemBatchAsync(context, context.AccessToken, context.ProjectId, context.PlayerId!, extractedData);
                return;
            case DataAccessibility.Protected:
                await gameApiClient.CloudSaveData.SetItemBatchAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId!, extractedData);
                return;
            case DataAccessibility.Private:
                await gameApiClient.CloudSaveData.SetProtectedItemBatchAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId!, extractedData);
                return;
            default:
                throw new InvalidOperationException();
        }
    }

    public static async Task<object?> LoadPlayerData(IExecutionContext context, IGameApiClient gameApiClient, DataAccessibility access, string key)
    {
        ApiResponse<GetItemsResponse> result = access switch
        {
            DataAccessibility.Public => await gameApiClient.CloudSaveData.GetPublicItemsAsync(context, context.AccessToken, context.ProjectId, context.PlayerId!, new List<string>() { key }),
            DataAccessibility.Protected => await gameApiClient.CloudSaveData.GetItemsAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId!, new List<string>() { key }),
            DataAccessibility.Private => await gameApiClient.CloudSaveData.GetProtectedItemsAsync(context, context.AccessToken, context.ProjectId, context.PlayerId!, new List<string>() { key }),
            _ => throw new InvalidOperationException()
        };

        return result.Data.Results.Count == 0 ? null : result.Data.Results.First().Value;
    }

    public static async Task<Dictionary<string, object>?> LoadMultiPlayerData(IExecutionContext context, IGameApiClient gameApiClient, DataAccessibility access, List<string> keys)
    {
        ApiResponse<GetItemsResponse> result = access switch
        {
            DataAccessibility.Public => await gameApiClient.CloudSaveData.GetPublicItemsAsync(context, context.AccessToken, context.ProjectId, context.PlayerId!, keys),
            DataAccessibility.Protected => await gameApiClient.CloudSaveData.GetItemsAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId!, keys),
            DataAccessibility.Private => await gameApiClient.CloudSaveData.GetProtectedItemsAsync(context, context.AccessToken, context.ProjectId, context.PlayerId!, keys),
            _ => throw new InvalidOperationException()
        };

        return result.Data.Results.Count == 0 ? null : result.Data.Results.ToDictionary(x => x.Key, x => x.Value);
    }

    public static async Task UpdatePlayerData(IExecutionContext context, IGameApiClient gameApiClient, DataAccessibility access, DataOperations op, string key, int value)
    {
        object? result = await LoadPlayerData(context, gameApiClient, access, key);
        
        if (result == null)
        {
            return;
        }

        result = op switch
        {
            DataOperations.Add => (int)result + value,
            DataOperations.Subtract => (int)result - value,
            DataOperations.Multiply => (int)result * value,
            DataOperations.Divide => (int)result / value,
            DataOperations.Update => value,
            _ => throw new InvalidOperationException()
        };

        await SavePlayerData(context, gameApiClient, access, key, result);
    }
    
    public static async Task UpdateMultiPlayerData(IExecutionContext context, IGameApiClient gameApiClient, DataAccessibility access, DataOperations op, Dictionary<string, int> data)
    {
        Dictionary<string, object> updatedData = new();
        Dictionary<string, object>? result = await LoadMultiPlayerData(context, gameApiClient, access, data.Select(x => x.Key).ToList());
        
        if (result == null)
        {
            return;
        }
        
        foreach (var (key, value) in result)
        {
            updatedData[key] = op switch
            {
                DataOperations.Add => (int)value + data[key],
                DataOperations.Subtract => (int)value - data[key],
                DataOperations.Multiply => (int)value * data[key],
                DataOperations.Divide => (int)value / data[key],
                DataOperations.Update => data[key],
                _ => throw new InvalidOperationException()
            };
        }
        
        await SaveMultiPlayerData(context, gameApiClient, access, updatedData);
    }
}