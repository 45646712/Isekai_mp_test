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
            nameof(ProtectedPlayerKeys.BalanceGold),
            nameof(ProtectedPlayerKeys.UnlockedCrops)
        };

        Dictionary<string, object>? publicData = await LoadMultiPlayerData(context, gameApiClient, DataAccessibility.Public, publicKeys);
        Dictionary<string, object>? protectedData = await LoadMultiPlayerData(context, gameApiClient, DataAccessibility.Protected, protectedKeys);

        Dictionary<string, object> updatedPublicData = new();
        Dictionary<string, object> updatedProtectedData = new();

        if (publicData != null)
        {
            foreach (string element in publicKeys.Where(element => !publicData.ContainsKey(element)))
            {
                updatedPublicData[element] = element switch
                {
                    nameof(PublicPlayerKeys.UserID) => await GenerateUserID(context, gameApiClient),
                    nameof(PublicPlayerKeys.Name) => publicData.TryGetValue(nameof(PublicPlayerKeys.UserID), out object? value) ? $"User_{value}" : await GenerateUserID(context, gameApiClient),
                    nameof(PublicPlayerKeys.Lv) => (byte)1,
                    nameof(PublicPlayerKeys.Exp) => 0,
                    _ => throw new InvalidOperationException()
                };
            }
        }
        else
        {
            updatedPublicData = new()
            {
                { nameof(PublicPlayerKeys.UserID), await GenerateUserID(context, gameApiClient) },
                { nameof(PublicPlayerKeys.Name), $"User_{await GenerateUserID(context, gameApiClient)}" },
                { nameof(PublicPlayerKeys.Lv), (byte)1 },
                { nameof(PublicPlayerKeys.Exp), 0 }
            };
        }

        if (protectedData != null)
        {
            foreach (string element in protectedKeys.Where(element => !protectedData.ContainsKey(element)))
            {
                updatedProtectedData[element] = element switch
                {
                    nameof(ProtectedPlayerKeys.BalanceGold) => 0,
                    nameof(ProtectedPlayerKeys.UnlockedCrops) => 20,
                    _ => throw new InvalidOperationException()
                };
            }
        }
        else
        {
            updatedProtectedData = new()
            {
                { nameof(ProtectedPlayerKeys.BalanceGold), 0 },
                { nameof(ProtectedPlayerKeys.UnlockedCrops), 20 }
            };
        }

        await SaveMultiPlayerData(context, gameApiClient, DataAccessibility.Public, updatedPublicData);
        await SaveMultiPlayerData(context, gameApiClient, DataAccessibility.Protected, updatedProtectedData);
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