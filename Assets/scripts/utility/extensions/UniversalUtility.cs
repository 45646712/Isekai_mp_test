using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using UnityEngine;
using static Constant.DataConstants;

namespace Extensions
{
    internal static class UniversalUtility
    {
        public static object DeserializeData(string key, string value) => key switch
        {
            nameof(PublicDataType.UserID) => long.Parse(value),
            nameof(PublicDataType.Name) => value,
            nameof(PublicDataType.Lv) => byte.Parse(value),
            nameof(PublicDataType.Exp) => int.Parse(value),
            nameof(ProtectedDataType.Inventory) => value,
            nameof(ProtectedDataType.BalanceGold) => int.Parse(value),
            nameof(ProtectedDataType.CropData) => value is null ? null : JsonSerializer.Deserialize<Dictionary<int, string>>(value),
            nameof(ProtectedDataType.UnlockedCropSlots) => int.Parse(value),
            nameof(ProtectedDataType.UnlockedFishSlots) => int.Parse(value),
            _ => throw new InvalidOperationException()
        };

        public static T GetEnumMaxIndex<T>(T EnumType) where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().Max();

        public static (int, int) IndexOf2LayerList<T>(List<List<T>> data, T source) =>
            data.SelectMany((x, i) => x.Select((y, j) => (Indices: (I: i, J: j), Value: y)))
                .Where(z => z.Value.Equals(source))
                .Select(z => z.Indices)
                .First();

        public static string FormatTime(int seconds)
        {
            if (seconds == 0)
            {
                return "0s";
            }
            
            TimeSpan duration = TimeSpan.FromSeconds(seconds);

            return duration.TotalSeconds switch
            {
                > 86400 => @$"{duration:d\d\:h\h\:m\m\:s\s}",
                > 3600 => @$"{duration:h\h\:m\m\:s\s}",
                > 60 => @$"{duration:m\m\:s\s}",
                > 0 => $"{duration.TotalSeconds}s",
                _ => throw new InvalidOperationException()
            };
        }
        
        public static string FormatTime(TimeSpan duration) => FormatTime((int)duration.TotalSeconds);
    }
}
