using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using Unity.Services.CloudSave.Model;

using static Data.DataConstants;
using static Data.PlayerData;

namespace Data;

public static class Account
{
    public static async Task ValidateAccountData(IExecutionContext context, IGameApiClient gameApiClient)
    {
        List<string> publicKeys = new()
        {
            nameof(PublicPlayerKeys.UserID),
            nameof(PublicPlayerKeys.Name),
            nameof(PublicPlayerKeys.Lv),
            nameof(PublicPlayerKeys.Exp)
        };
        
        List<string> protectedKeys = new()
        {
            nameof(ProtectedPlayerKeys.Currency_Gold),
            nameof(ProtectedPlayerKeys.UnlockedCropSlots),
            nameof(ProtectedPlayerKeys.UnlockedFishSlots)
        };

        Dictionary<string, object> publicData = await LoadMultiPlayerData(context, gameApiClient, DataAccessibility.Public, publicKeys) ?? new();
        Dictionary<string, object> protectedData = await LoadMultiPlayerData(context, gameApiClient, DataAccessibility.Protected, protectedKeys) ?? new();
        
        publicData.TryAdd(nameof(PublicPlayerKeys.UserID), await GenerateUserID(context, gameApiClient));
        publicData.TryAdd(nameof(PublicPlayerKeys.Name), $"User_{publicData[nameof(PublicPlayerKeys.UserID)]}");
        publicData.TryAdd(nameof(PublicPlayerKeys.Lv), (byte)1);
        publicData.TryAdd(nameof(PublicPlayerKeys.Exp), 0);
        
        protectedData.TryAdd(nameof(ProtectedPlayerKeys.Currency_Gold), 0);
        protectedData.TryAdd(nameof(ProtectedPlayerKeys.UnlockedCropSlots), 5);
        protectedData.TryAdd(nameof(ProtectedPlayerKeys.UnlockedFishSlots), 2);

        await SaveMultiPlayerData(context, gameApiClient, DataAccessibility.Public, publicData);
        await SaveMultiPlayerData(context, gameApiClient, DataAccessibility.Protected, protectedData);
    }

    private static async Task<Int64> GenerateUserID(IExecutionContext context, IGameApiClient gameApiClient)
    {
        try
        {
            QueryIndexBody query = new QueryIndexBody(new List<FieldFilter> { new("UserID", Int64.MaxValue, FieldFilter.OpEnum.LE) }, new List<string> { "UserID" }, 0, 1);

            ApiResponse<QueryIndexResponse> result = await gameApiClient.CloudSaveData.QueryPublicPlayerDataAsync(context, context.ServiceToken, context.ProjectId, query);

            return result.Data.Results.Count == 0 ? 10000000 : (Int64)result.Data.Results.First().Data.First().Value + 1;
        }
        catch (ApiException ex)
        {
            throw new Exception($"Failed to save data for playerId {context.PlayerId}. Error: {ex.Message}");
        }
    }
}