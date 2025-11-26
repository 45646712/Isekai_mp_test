using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

using static Data.DataConstants;
using static Data.CropConstants;

namespace Data;

public class GameDataEndpoints
{
    [CloudCodeFunction("LoadGameData")]
    public async Task<string?> LoadGameData(IExecutionContext context, IGameApiClient gameApiClient, GameDataType type, int ID) => await GameData.LoadGameData(context, gameApiClient, type, ID);
    
    [CloudCodeFunction("LoadMultiGameData")]
    public async Task<List<string>?> LoadMultiGameData(IExecutionContext context, IGameApiClient gameApiClient, GameDataType type)
    {
        List<string> result = await GameData.LoadMultiGameData(context, gameApiClient, type);

        return result.Count == 0 ? null : result.Select(x => x).ToList();
    }
}