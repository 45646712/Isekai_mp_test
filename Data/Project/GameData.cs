using System;
using System.Text.Json;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

using static Data.DataConstants;
using static Data.CropModels;
using static Data.CropConstants;

namespace Data;

public class GameData
{
    public static async Task SaveGameData(IExecutionContext context, IGameApiClient gameApiClient, GameDataType type, string ID, string data)
    {
        SetItemBatchBody body = type switch
        {
            GameDataType.Crop => new Crop(data).ConvertToGameData()
        };

        await gameApiClient.CloudSaveData.SetCustomItemBatchAsync(context, context.ServiceToken, context.ProjectId, ID, body);
    }
}