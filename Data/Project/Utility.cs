using System;
using Microsoft.Extensions.DependencyInjection;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

using static Data.DataConstants;

namespace Data;

public static class Utility
{
    public static object DeserializeData(string key, string value) => key switch
    {
        nameof(PublicPlayerKeys.UserID) => long.Parse(value),
        nameof(PublicPlayerKeys.Name) => value,
        nameof(PublicPlayerKeys.Lv) => byte.Parse(value),
        nameof(PublicPlayerKeys.Exp) => int.Parse(value),
        nameof(ProtectedPlayerKeys.Inventory) => value,
        nameof(ProtectedPlayerKeys.BalanceGold) => int.Parse(value),
        nameof(ProtectedPlayerKeys.CropData) => value,
        nameof(ProtectedPlayerKeys.UnlockedCrops) => int.Parse(value),
        _ => throw new InvalidOperationException()
    };
}

public class ModuleConfig : ICloudCodeSetup
{
    public void Setup(ICloudCodeConfig config)
    {
        config.Dependencies.AddSingleton(GameApiClient.Create());
    }
}