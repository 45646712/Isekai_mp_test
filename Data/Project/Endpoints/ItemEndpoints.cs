using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

using static Data.ItemConstants;
using static Data.DataConstants;

namespace Data.Endpoints;

public class ItemEndpoints
{
    [CloudCodeFunction("UpdateItem")]
    public async Task UpdateItem(IExecutionContext context, IGameApiClient gameApiClient, DataOperations op, ItemType type, int itemID, int amount) => await Item.UpdateItem(context, gameApiClient, op, type, itemID, amount);

    [CloudCodeFunction("UpdateMultiItem")]
    public async Task UpdateMultiItem(IExecutionContext context, IGameApiClient gameApiClient, DataOperations op, ItemType type, Dictionary<string, int> data) => await Item.UpdateMultiItem(context, gameApiClient, op, type, data.ToDictionary(x => int.Parse(x.Key), x => x.Value));
}