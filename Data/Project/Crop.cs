using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using static Data.ItemConstants;
using static Data.CropModels;
using static Data.DataConstants;

namespace Data;

public class Crop
{
    public static async Task Plant(IExecutionContext context, IGameApiClient gameApiClient, int slotID, int gamedataID)
    {
        await ValidateProgress(context, gameApiClient, slotID);
        
        CropBaseData baseData = JsonSerializer.Deserialize<CropBaseData>(await GameData.LoadGameData(context, gameApiClient, GameDataType.Crop, gamedataID) ?? string.Empty) ?? throw new NullReferenceException("game data not found!");
        
        string? result = (string?)await PlayerData.LoadPlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.CropData));

        Dictionary<int, string> data = result is null ? new() : JsonSerializer.Deserialize<Dictionary<int, string>>(result)!;

        data[slotID] = data.ContainsKey(slotID) ? throw new InvalidOperationException("Slot is not empty!") : JsonSerializer.Serialize(new CropUploadData(baseData.ID, DateTimeOffset.UtcNow.AddSeconds(baseData.TimeNeeded)));
        
        await PlayerData.UpdateMultiPlayerData(context, gameApiClient, DataAccessibility.Protected, DataOperations.Subtract, baseData.Costs.ToDictionary(x => x.Key.ToString(), x => x.Value));
        await PlayerData.SavePlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.CropData), JsonSerializer.Serialize(data));
    }

    public static async Task MultiPlant(IExecutionContext context, IGameApiClient gameApiClient, List<int> slotIDs, int gamedataID)
    {
        await ValidateProgress(context, gameApiClient, slotIDs.Max());
        
        CropBaseData baseData = JsonSerializer.Deserialize<CropBaseData>(await GameData.LoadGameData(context, gameApiClient, GameDataType.Crop, gamedataID) ?? string.Empty) ?? throw new NullReferenceException("game data not found!");
        
        string? result = (string?)await PlayerData.LoadPlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.CropData));

        Dictionary<int, string> data = result is null ? new() : JsonSerializer.Deserialize<Dictionary<int, string>>(result)!;

        foreach (int element in slotIDs)
        {
            data[element] = data.ContainsKey(element) ? throw new InvalidOperationException("Slot is not empty!") : JsonSerializer.Serialize(new CropUploadData(baseData.ID, DateTimeOffset.UtcNow.AddSeconds(baseData.TimeNeeded)));
        }

        await PlayerData.UpdateMultiPlayerData(context, gameApiClient, DataAccessibility.Protected, DataOperations.Subtract, baseData.Costs.ToDictionary(x => x.Key.ToString(), x => x.Value * slotIDs.Count));
        await PlayerData.SavePlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.CropData), JsonSerializer.Serialize(data));
    }

    public static async Task Harvest(IExecutionContext context, IGameApiClient gameApiClient, int slotID)
    {
        await ValidateProgress(context, gameApiClient, slotID);
        
        string? result = (string?)await PlayerData.LoadPlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.CropData));

        Dictionary<int, string> source = result is null ? throw new InvalidOperationException("no valid slots for harvesting!") : JsonSerializer.Deserialize<Dictionary<int, string>>(result)!;
        CropUploadData data = source.TryGetValue(slotID, out string? value) ? JsonSerializer.Deserialize<CropUploadData>(value)! : throw new InvalidOperationException("slot is not valid for harvesting!");
        
        CropBaseData baseData = JsonSerializer.Deserialize<CropBaseData>(await GameData.LoadGameData(context, gameApiClient, GameDataType.Crop, data.CropID) ?? string.Empty) ?? throw new NullReferenceException("game data not found!");

        if (data.MatureTime > DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException("slot is not valid for harvesting!");
        }
        
        source.Remove(slotID);

        await Item.UpdateItem(context, gameApiClient, DataOperations.Add, ItemType.Crop, baseData.ID, baseData.Rewards[ResourceType.Item]);
        await PlayerData.UpdateMultiPlayerData(context, gameApiClient, DataAccessibility.Public, DataOperations.Add, baseData.Rewards.ToDictionary(x => x.Key.ToString(), x => x.Value));
        await PlayerData.UpdateMultiPlayerData(context, gameApiClient, DataAccessibility.Protected, DataOperations.Add, baseData.Rewards.ToDictionary(x => x.Key.ToString(), x => x.Value));
        await PlayerData.SavePlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.CropData), JsonSerializer.Serialize(source));
    }
    
    public static async Task MultiHarvest(IExecutionContext context, IGameApiClient gameApiClient, List<int> slotIDs)
    {
        await ValidateProgress(context, gameApiClient, slotIDs.Max());
        
        string? result = (string?)await PlayerData.LoadPlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.CropData));

        Dictionary<int, string> source = result is null ? throw new InvalidOperationException("no valid slots for harvesting!") : JsonSerializer.Deserialize<Dictionary<int, string>>(result)!;
        
        Dictionary<int, CropUploadData> allTarget = source.Count == 0 ? throw new InvalidOperationException("no valid slots for harvesting!") : slotIDs.ToDictionary(x => x, x => JsonSerializer.Deserialize<CropUploadData>(source[x] ?? throw new InvalidOperationException("slot is not harvestable!"))!);
        
        List<string> baseSource = await GameData.LoadMultiGameData(context, gameApiClient, GameDataType.Crop);
        List<CropBaseData> AllBaseData = baseSource.Select(x => JsonSerializer.Deserialize<CropBaseData>(x)!).OrderBy(y => y.ID).ToList();

        Dictionary<string, int> totalRewards = new();
        Dictionary<int, int> totalItemRewards = new();

        foreach (var (slot, value) in allTarget.Where(x => x.Value.MatureTime < DateTimeOffset.UtcNow))
        {
            foreach (var (type, amount) in AllBaseData[value.CropID - 1].Rewards)
            {
                if (type == ResourceType.Item)
                {
                    totalItemRewards[value.CropID] = totalItemRewards.TryGetValue(value.CropID, out int itemReward) ? itemReward + amount : amount;
                }

                totalRewards[type.ToString()] = totalRewards.TryGetValue(type.ToString(), out int statReward) ? statReward + amount : amount;
            }

            source.Remove(slot);
        }
        
        await Item.UpdateMultiItem(context, gameApiClient, DataOperations.Add, ItemType.Crop, totalItemRewards);
        await PlayerData.UpdateMultiPlayerData(context, gameApiClient, DataAccessibility.Public, DataOperations.Add, totalRewards);
        await PlayerData.UpdateMultiPlayerData(context, gameApiClient, DataAccessibility.Protected, DataOperations.Add, totalRewards);
        await PlayerData.SavePlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.CropData), JsonSerializer.Serialize(source));
    }

    public static async Task Remove(IExecutionContext context, IGameApiClient gameApiClient, int slotID)
    {
        await ValidateProgress(context, gameApiClient, slotID);
        
        string? result = (string?)await PlayerData.LoadPlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.CropData));

        Dictionary<int, string> data = result is null ? throw new InvalidOperationException("no valid slots for removing!") : JsonSerializer.Deserialize<Dictionary<int, string>>(result)!;
        
        data.Remove(slotID);
        
        await PlayerData.SavePlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.CropData), JsonSerializer.Serialize(data));
    }
    
    public static async Task MultiRemove(IExecutionContext context, IGameApiClient gameApiClient, List<int> slotIDs)
    {
        await ValidateProgress(context, gameApiClient, slotIDs.Max());
        
        string? result = (string?)await PlayerData.LoadPlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.CropData));

        Dictionary<int, string> data = result is null ? throw new InvalidOperationException("no valid slots for removing!") : JsonSerializer.Deserialize<Dictionary<int, string>>(result)!;
        
        foreach (int element in slotIDs)
        {
            data.Remove(element);
        }
        
        await PlayerData.SavePlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.CropData), JsonSerializer.Serialize(data));
    }

    public static async Task<string?> TrackCropUpdate(IExecutionContext context, IGameApiClient gameApiClient)
    {
        string? result = (string?)await PlayerData.LoadPlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.CropData));
        
        Dictionary<int, string>? source = result is null ? null : JsonSerializer.Deserialize<Dictionary<int, string>>(result)!;

        if (source == null || source.Count == 0)
        {
            return null;
        }

        DateTimeOffset nextUpdateTime = source
            .Select(x => JsonSerializer.Deserialize<CropUploadData>(x.Value)!.MatureTime)
            .Where(x => x > DateTimeOffset.UtcNow).OrderBy(x => x).FirstOrDefault();
        
        return JsonSerializer.Serialize(nextUpdateTime);
    }

    private static async Task ValidateProgress(IExecutionContext context, IGameApiClient gameApiClient, int slotID)
    {
        long? unlockProgress = (long?)await PlayerData.LoadPlayerData(context, gameApiClient, DataAccessibility.Protected, nameof(ProtectedPlayerKeys.UnlockedCropSlots));

        if (slotID >= unlockProgress)
        {
            throw new InvalidOperationException("request passed unlocked amount, potential manipulation!");
        }
    }
}