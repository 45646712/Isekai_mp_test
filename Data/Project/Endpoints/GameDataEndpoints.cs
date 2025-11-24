using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

using static Data.DataConstants;
using static Data.CropConstants;

namespace Data;

public class GameDataEndpoints
{
    [CloudCodeFunction("SaveGameData")]
    public async Task SaveGameData(IExecutionContext context, IGameApiClient gameApiClient, GameDataType type, string ID, string data) => await GameData.SaveGameData(context, gameApiClient, type, ID, data);
    
    [CloudCodeFunction("LoadGameData")]
    public async Task<string?> LoadPlayerData(IExecutionContext context, IGameApiClient gameApiClient, DataConstants.DataAccessibility access, string key) => (string?)await PlayerData.LoadPlayerData(context, gameApiClient, access, key);
}