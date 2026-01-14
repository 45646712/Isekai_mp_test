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
        CropBaseData crop1 = new CropBaseData(1, "Test_Grass", ItemCategory.Materials, "test purpose", 10,
            new()
            {
                { ResourceType.Currency_Gold, 5 }
            }, new()
            {
                { ResourceType.Exp, 2 },
                { ResourceType.Item, 4 }
            },
            new()
            {
                { CropStatus.Growing, "4a0f991f-f543-4297-ad74-0c4fa7b45252" },
                { CropStatus.Matured, "5cd25a19-b298-4aec-a2af-a3322fc70e4b" }
            }, new()
            {
                { CropStatus.Growing, new string[] { } },
                { CropStatus.Matured, new string[] { } }
            },
            "d18e9b36-7025-4d3d-9ca2-33baffcc5dd5",
            "c91d2c40-f5f8-4f30-bb2f-1f4725cdccf7",
            "32bc23ea-f68c-4e2a-b64c-997f8773a7c6");

        CropBaseData crop2 = new CropBaseData(2, "Test_Grass2", ItemCategory.Materials, "test purpose2", 11,
            new()
            {
                { ResourceType.Currency_Gold, 6 }
            }, new()
            {
                { ResourceType.Exp, 3 },
                { ResourceType.Item, 5 }
            },
            new()
            {
                { CropStatus.Growing, "4a0f991f-f543-4297-ad74-0c4fa7b45252" },
                { CropStatus.Matured, "5cd25a19-b298-4aec-a2af-a3322fc70e4b" }
            }, new()
            {
                { CropStatus.Growing, new string[] { } },
                { CropStatus.Matured, new string[] { } }
            },
            "d18e9b36-7025-4d3d-9ca2-33baffcc5dd5",
            "c91d2c40-f5f8-4f30-bb2f-1f4725cdccf7",
            "32bc23ea-f68c-4e2a-b64c-997f8773a7c6");

        CropBaseData crop3 = new CropBaseData(3, "Test_Grass3", ItemCategory.Materials, "test purpose3", 12,
            new()
            {
                { ResourceType.Currency_Gold, 7 }
            }, new()
            {
                { ResourceType.Exp, 4 },
                { ResourceType.Item, 6 }
            },
            new()
            {
                { CropStatus.Growing, "4a0f991f-f543-4297-ad74-0c4fa7b45252" },
                { CropStatus.Matured, "5cd25a19-b298-4aec-a2af-a3322fc70e4b" }
            }, new()
            {
                { CropStatus.Growing, new string[] { } },
                { CropStatus.Matured, new string[] { } }
            }, 
            "d18e9b36-7025-4d3d-9ca2-33baffcc5dd5",
            "c91d2c40-f5f8-4f30-bb2f-1f4725cdccf7", 
            "32bc23ea-f68c-4e2a-b64c-997f8773a7c6");

        CropBaseData crop4 = new CropBaseData(4, "Test_Grass4", ItemCategory.Materials, "test purpose4", 13,
            new()
            {
                { ResourceType.Currency_Gold, 8 }
            }, new()
            {
                { ResourceType.Exp, 5 },
                { ResourceType.Item, 7 }
            },
            new()
            {
                { CropStatus.Growing, "4a0f991f-f543-4297-ad74-0c4fa7b45252" },
                { CropStatus.Matured, "5cd25a19-b298-4aec-a2af-a3322fc70e4b" }
            }, new()
            {
                { CropStatus.Growing, new string[] { } },
                { CropStatus.Matured, new string[] { } }
            },
            "d18e9b36-7025-4d3d-9ca2-33baffcc5dd5",
            "c91d2c40-f5f8-4f30-bb2f-1f4725cdccf7",
            "32bc23ea-f68c-4e2a-b64c-997f8773a7c6");

        CropBaseData crop5 = new CropBaseData(5, "Test_Grass5", ItemCategory.Materials, "test purpose5", 14,
            new()
            {
                { ResourceType.Currency_Gold, 9 }
            }, new()
            {
                { ResourceType.Exp, 6 },
                { ResourceType.Item, 8 }
            },
            new()
            {
                { CropStatus.Growing, "4a0f991f-f543-4297-ad74-0c4fa7b45252" },
                { CropStatus.Matured, "5cd25a19-b298-4aec-a2af-a3322fc70e4b" }
            }, new()
            {
                { CropStatus.Growing, new string[] { } },
                { CropStatus.Matured, new string[] { } }
            },
            "d18e9b36-7025-4d3d-9ca2-33baffcc5dd5",
            "c91d2c40-f5f8-4f30-bb2f-1f4725cdccf7",
            "32bc23ea-f68c-4e2a-b64c-997f8773a7c6");

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