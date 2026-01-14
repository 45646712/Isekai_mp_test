using System;
using Microsoft.Extensions.DependencyInjection;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Apis.Admin;

using static Data.DataConstants;

namespace Data;

public static class Utility
{
    public static object DeserializeData(string key, string value) => key switch
    {
        nameof(PublicPlayerKeys.UserID) => long.Parse(value),
        nameof(PublicPlayerKeys.Name) => value,
        nameof(PublicPlayerKeys.Lv) => long.Parse(value),
        nameof(PublicPlayerKeys.Exp) => long.Parse(value),
        nameof(ProtectedPlayerKeys.Inventory) => value,
        nameof(ProtectedPlayerKeys.Currency_Gold) => long.Parse(value),
        nameof(ProtectedPlayerKeys.CropData) => value,
        nameof(ProtectedPlayerKeys.UnlockedCropSlots) => long.Parse(value),
        nameof(ProtectedPlayerKeys.UnlockedFishSlots) => long.Parse(value),
        _ => throw new InvalidOperationException()
    };
}

public class ModuleConfig : ICloudCodeSetup
{
    public void Setup(ICloudCodeConfig config)
    {
        config.Dependencies.AddSingleton(GameApiClient.Create());
        config.Dependencies.AddSingleton(AdminApiClient.Create());
    }
}