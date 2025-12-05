using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

using static Data.DataConstants;
using static Data.PlayerData;
using static Data.Utility;

namespace Data.Endpoints;

public class PlayerDataEndpoints
{
    [CloudCodeFunction("SavePlayerData")]
    public async Task SavePlayerData(IExecutionContext context, IGameApiClient gameApiClient, DataAccessibility access, string key, string value)
    {
        object deserializedValue = access switch
        {
            DataAccessibility.Public => DeserializeData(key, value),
            DataAccessibility.Protected => DeserializeData(key, value),
            _ => throw new InvalidOperationException()
        };

        await PlayerData.SavePlayerData(context, gameApiClient, access, key, deserializedValue);
    }

    [CloudCodeFunction("SaveMultiPlayerData")]
    public async Task SaveMultiPlayerData(IExecutionContext context, IGameApiClient gameApiClient, DataAccessibility access, Dictionary<string, string> data)
    {
        Dictionary<string, object> deserializedData = new();

        foreach (var (key, value)in data)
        {
            deserializedData[key] = access switch
            {
                DataAccessibility.Public => DeserializeData(key, value),
                DataAccessibility.Protected => DeserializeData(key, value),
                _ => throw new InvalidOperationException()
            };
        }

        await PlayerData.SaveMultiPlayerData(context, gameApiClient, access, deserializedData);
    }

    [CloudCodeFunction("LoadPlayerData")]
    public async Task<string?> LoadPlayerData(IExecutionContext context, IGameApiClient gameApiClient, DataAccessibility access, string key) => (string?)await PlayerData.LoadPlayerData(context, gameApiClient, access, key);

    [CloudCodeFunction("LoadMultiPlayerData")]
    public async Task<Dictionary<string,string?>?> LoadMultiPlayerData(IExecutionContext context, IGameApiClient gameApiClient, DataAccessibility access, List<string> keys)
    {
        Dictionary<string, object>? result = await PlayerData.LoadMultiPlayerData(context, gameApiClient, access, keys);

        return result?.ToDictionary(x => x.Key, x => x.Value.ToString());
    }
    
    [CloudCodeFunction("UpdatePlayerData")]
    public async Task UpdatePlayerData(IExecutionContext context, IGameApiClient gameApiClient, DataAccessibility access, DataOperations op, string key, int value) => await UpdatePlayerData(context, gameApiClient, access, op, key, value);
    
    [CloudCodeFunction("UpdateMultiPlayerData")]
    public async Task UpdateMultiPlayerData(IExecutionContext context, IGameApiClient gameApiClient, DataAccessibility access, DataOperations op, Dictionary<string, int> data) => await UpdateMultiPlayerData(context, gameApiClient, access, op, data);
}