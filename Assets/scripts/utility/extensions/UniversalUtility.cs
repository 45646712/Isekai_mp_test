using System;
using System.Linq;
using UnityEngine;

namespace Extensions
{
    static class UniversalUtility
    {
        public static T GetEnumMaxIndex<T>(T EnumType) where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().Max();
    }
}
