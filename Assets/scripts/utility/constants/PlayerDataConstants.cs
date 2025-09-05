using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Constant
{
    public static class PlayerDataConstants
    {
        public enum DataAccessibility
        {
            Public,
            Protected,
            Private
        }

        public enum PublicDataType //public data 
        {
            UserID, //int64
            Name,//string
            Lv, //byte
            Exp //int
        }

        public enum ProtectedDataType //default data
        {
            Inventory, //string
            BalanceGold, //int
            CropData, //string
            UnlockedCrops,
        }

        public enum PrivateDataType //private data
        {
            
        }

        public static HashSet<string> GetKey<T>(T key) where T : Enum => new() { key.ToString() };

        public static HashSet<string> GetKeys<T>(List<T> keys) where T : Enum
        {
            HashSet<string> allKeys = new();

            foreach (T element in keys)
            {
                allKeys.Add(element.ToString());
            }

            return allKeys;
        }

        public static List<T> GetEnumSet<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().ToList();
    }
}