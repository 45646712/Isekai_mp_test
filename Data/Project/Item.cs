using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

using static Data.ItemConstants;
using static Data.DataConstants;

namespace Data;

public class Item //TODO: get + getmulti
{
    public static async Task UpdateItem(IExecutionContext context, IGameApiClient gameApiClient, DataOperations op, ItemType type, int itemID, int amount)
    {
        string? result = (string?)await PlayerData.LoadPlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.Inventory));

        Dictionary<ItemType, Dictionary<int, int>> source = result is null ? new() : JsonSerializer.Deserialize<Dictionary<ItemType, Dictionary<int, int>>>(result)!;

        source.TryAdd(type, new());

        source[type][itemID] = op switch
        {
            DataOperations.Add => source[type].ContainsKey(itemID) ? source[type][itemID] + amount : amount,
            DataOperations.Subtract => source[type].ContainsKey(itemID) ? source[type][itemID] - amount : amount,
            DataOperations.Multiply => source[type].ContainsKey(itemID) ? source[type][itemID] * amount : amount,
            DataOperations.Divide => source[type].ContainsKey(itemID) ? source[type][itemID] / amount : amount,
            DataOperations.Update => source[type].ContainsKey(itemID) ? source[type][itemID] : amount,
            _ => throw new InvalidOperationException("invalid data operation!")
        };

        if (source[type][itemID] < 0)
        {
            throw new InvalidOperationException("Insufficient Item!");
        }

        await PlayerData.SavePlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.Inventory), JsonSerializer.Serialize(source));
    }

    public static async Task UpdateMultiItem(IExecutionContext context, IGameApiClient gameApiClient, DataOperations op, ItemType type, Dictionary<int, int> data)
    {
        string? result = (string?)await PlayerData.LoadPlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.Inventory));

        Dictionary<ItemType, Dictionary<int, int>> source = result is null ? new() : JsonSerializer.Deserialize<Dictionary<ItemType, Dictionary<int, int>>>(result)!;

        source.TryAdd(type, new());

        foreach (var (key, value) in data)
        {
            source[type][key] = op switch
            {
                DataOperations.Add => source[type].ContainsKey(key) ? source[type][key] + value : value,
                DataOperations.Subtract => source[type].ContainsKey(key) ? source[type][key] - value : value,
                DataOperations.Multiply => source[type].ContainsKey(key) ? source[type][key] * value : value,
                DataOperations.Divide => source[type].ContainsKey(key) ? source[type][key] / value : value,
                DataOperations.Update => source[type].ContainsKey(key) ? source[type][key] : value,
                _ => throw new InvalidOperationException("invalid data operation!")
            };

            if (source[type][key] < 0)
            {
                throw new InvalidOperationException("Insufficient Item!");
            }
        }

        await PlayerData.SavePlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.Inventory), JsonSerializer.Serialize(source));
    }
}