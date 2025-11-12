using System;
using UnityEngine;

namespace Constant
{
    public static class ItemConstants
    {
        public enum ItemType // crop/fish/building/store ID -> itemID
        {
            Crop, //ID 1~
            Fish, //ID 5001~
            Building //ID 10001~
        }

        public enum ItemUpdateOperation
        {
            Update,
            Add,
            Subtract
        }
        
        public enum ItemCategory
        {
            Materials = 1,
            Resources,
            Valuables,
            Quest,
            Exotics,
            Others
        }

        public enum ResourceType
        {
            Exp,
            Item,
            Currency_Gold,
            Time,
        }
    }
}