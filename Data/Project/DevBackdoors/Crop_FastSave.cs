using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

using static Data.DataConstants;
using static Data.CropModels;
using static Data.CropConstants;
using static Data.ItemConstants;

namespace Data;

public static class Crop_FastSave
{
    public static async Task Dev_BatchSaveCropData(IExecutionContext context, IGameApiClient gameApiClient)
    {
        Crop crop1 = new Crop(1, "Test_Grass", ItemCategory.Materials, "test purpose", new()
            {
                { ResourceType.Currency_Gold, 5 },
                { ResourceType.Time, 10 }
            }, new()
            {
                { ResourceType.Exp, 2 },
                { ResourceType.Item, 4 }
            },
            new()
            {
                { CropStatus.Growing, "1" },
                { CropStatus.Matured, "2" }
            }, new()
            {
                { CropStatus.Growing, new[] { "1" } }
            }, "1", "2", "3");

        Crop crop2 = new Crop(2, "Test_Grass2", ItemCategory.Materials, "test purpose2", new()
            {
                { ResourceType.Currency_Gold, 6 },
                { ResourceType.Time, 11 }
            }, new()
            {
                { ResourceType.Exp, 3 },
                { ResourceType.Item, 5 }
            },
            new()
            {
                { CropStatus.Growing, "1" },
                { CropStatus.Matured, "2" }
            }, new()
            {
                { CropStatus.Growing, new[] { "1" } }
            }, "11", "12", "13");

        Crop crop3 = new Crop(3, "Test_Grass3", ItemCategory.Materials, "test purpose3", new()
            {
                { ResourceType.Currency_Gold, 7 },
                { ResourceType.Time, 12 }
            }, new()
            {
                { ResourceType.Exp, 4 },
                { ResourceType.Item, 6 }
            },
            new()
            {
                { CropStatus.Growing, "1" },
                { CropStatus.Matured, "2" }
            }, new()
            {
                { CropStatus.Growing, new[] { "1" } }
            }, "21", "22", "23");

        Crop crop4 = new Crop(4, "Test_Grass4", ItemCategory.Materials, "test purpose4", new()
            {
                { ResourceType.Currency_Gold, 8 },
                { ResourceType.Time, 13 }
            }, new()
            {
                { ResourceType.Exp, 5 },
                { ResourceType.Item, 7 }
            },
            new()
            {
                { CropStatus.Growing, "1" },
                { CropStatus.Matured, "2" }
            }, new()
            {
                { CropStatus.Growing, new[] { "1" } }
            }, "31", "32", "33");
        
        Crop crop5 = new Crop(5, "Test_Grass5", ItemCategory.Materials, "test purpose5", new()
            {
                { ResourceType.Currency_Gold, 9 },
                { ResourceType.Time, 14 }
            }, new()
            {
                { ResourceType.Exp, 6 },
                { ResourceType.Item, 8 }
            },
            new()
            {
                { CropStatus.Growing, "1" },
                { CropStatus.Matured, "2" }
            }, new()
            {
                { CropStatus.Growing, new[] { "1" } }
            }, "41", "42", "43");

        SetItemBatchBody query = new SetItemBatchBody(new()
        {
            new SetItemBody(crop1.ID.ToString(), JsonSerializer.Serialize(crop1)),
            new SetItemBody(crop2.ID.ToString(), JsonSerializer.Serialize(crop2)),
            new SetItemBody(crop3.ID.ToString(), JsonSerializer.Serialize(crop3)),
            new SetItemBody(crop4.ID.ToString(), JsonSerializer.Serialize(crop4)),
            new SetItemBody(crop5.ID.ToString(), JsonSerializer.Serialize(crop5))
        });

        await gameApiClient.CloudSaveData.SetCustomItemBatchAsync(context, context.ServiceToken, context.ProjectId, nameof(GameDataType.Crop), query);
    }
}