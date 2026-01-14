using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

namespace Data.Endpoints;

public class CropEndpoints
{
    [CloudCodeFunction("Plant")]
    public async Task Plant(IExecutionContext context, IGameApiClient gameApiClient, int slotID, int gamedataID) => await Crop.Plant(context, gameApiClient, slotID, gamedataID);

    [CloudCodeFunction("MultiPlant")]
    public async Task MultiPlant(IExecutionContext context, IGameApiClient gameApiClient, List<int> slotIDs, int gamedataID) => await Crop.MultiPlant(context, gameApiClient, slotIDs, gamedataID);

    [CloudCodeFunction("Harvest")]
    public async Task Harvest(IExecutionContext context, IGameApiClient gameApiClient, int slotID) => await Crop.Harvest(context, gameApiClient, slotID);

    [CloudCodeFunction("MultiHarvest")]
    public async Task MultiHarvest(IExecutionContext context, IGameApiClient gameApiClient, List<int> slotIDs) => await Crop.MultiHarvest(context, gameApiClient, slotIDs);

    [CloudCodeFunction("Remove")]
    public async Task Remove(IExecutionContext context, IGameApiClient gameApiClient, int slotID) => await Crop.Remove(context, gameApiClient, slotID);

    [CloudCodeFunction("MultiRemove")]
    public async Task MultiRemove(IExecutionContext context, IGameApiClient gameApiClient, List<int> slotIDs) => await Crop.MultiRemove(context, gameApiClient, slotIDs);

    [CloudCodeFunction("TrackCropUpdate")]
    public async Task<string?> TrackCropUpdate(IExecutionContext context, IGameApiClient gameApiClient) => await Crop.TrackCropUpdate(context, gameApiClient);
}