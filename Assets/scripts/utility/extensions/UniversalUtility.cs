using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Extensions
{
    internal static class UniversalUtility
    {
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

            return duration.Seconds switch
            {
                > 86400 => @$"{duration:d\:h\:m\:s}",
                > 3600 => @$"{duration:h\:m\:s\}",
                > 60 => @$"{duration:m\:s\}",
                > 0 => $"{duration}s",
                _ => throw new InvalidOperationException()
            };
        }
        
        public static string FormatTime(TimeSpan duration) => FormatTime((int)duration.TotalSeconds);
    }
}
