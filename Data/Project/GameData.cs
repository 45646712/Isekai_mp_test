using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;

using static Data.DataConstants;
using static Data.CropConstants;
using static Data.ItemConstants;
using static Data.CropModels;

namespace Data;

public static class GameData
{ 
    public static async Task<string?> LoadGameData(IExecutionContext context, IGameApiClient gameApiClient, GameDataType type, int ID)
    {
        ApiResponse<GetItemsResponse> result = await gameApiClient.CloudSaveData.GetCustomItemsAsync(context, context.AccessToken, context.ProjectId, type.ToString());

        return (string?)result.Data.Results.FirstOrDefault(x => x.Key == ID.ToString())?.Value;
    }
    
    public static async Task<List<string>> LoadMultiGameData(IExecutionContext context, IGameApiClient gameApiClient, GameDataType type)
    {
        ApiResponse<GetItemsResponse> result = await gameApiClient.CloudSaveData.GetCustomItemsAsync(context, context.AccessToken, context.ProjectId, type.ToString());

        return result.Data.Results.Select(x => (string)x.Value).ToList();
    }
}