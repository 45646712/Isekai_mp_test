using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Constant
{
    public static class PlayerDataConstant
    {
        public enum PublicDataType //public data 
        {
            UserID, //int64
            Lv, //byte
            Name
        }

        public enum ProtectedDataType //default data
        {
            
        }
        
        public enum PrivateDataType //private data
        {
            
        }

        public static HashSet<string> PublicKeys
        {
            get
            { 
                HashSet<string> allKeys = new();
                
                foreach (var element in Enum.GetValues(typeof(PublicDataType)))
                {
                    allKeys.Add(element.ToString());
                }
                
                return allKeys;
            }
        }
        
        public static HashSet<string> ProtectedKeys
        {
            get
            { 
                HashSet<string> allKeys = new();
                
                foreach (var element in Enum.GetValues(typeof(ProtectedDataType)))
                {
                    allKeys.Add(element.ToString());
                }
                
                return allKeys;
            }
        }
        
        public static HashSet<string> PrivateKeys
        {
            get
            { 
                HashSet<string> allKeys = new();
                
                foreach (var element in Enum.GetValues(typeof(PrivateDataType)))
                {
                    allKeys.Add(element.ToString());
                }
                
                return allKeys;
            }
        }
    }
}