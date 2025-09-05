using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Constant
{
    public static class GameDataConstants
    {
        public enum DataType //public data 
        {
            test
        }

        public static HashSet<string> Keys
        {
            get
            { 
                HashSet<string> allKeys = new();
                
                foreach (var element in Enum.GetValues(typeof(DataType)))
                {
                    allKeys.Add(element.ToString());
                }
                
                return allKeys;
            }
        }
        
        public static HashSet<string> ConvertQueryToKeys<T>(List<T> types) // only accept enum
        {
            HashSet<string> allKeys = new();

            foreach (T element in types)
            {
                allKeys.Add(element.ToString());
            }
            
            return allKeys;
        }
    }
}