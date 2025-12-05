using System;
using System.Text.Json.Serialization;
using Constant;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Models
{
    public static class ItemModel
    {
        public class ItemBaseData
        {
            // general data
            [JsonInclude] public int ID;
            [JsonInclude] public string Name;
            [JsonInclude] public ItemConstants.ItemCategory Category;
            [JsonInclude] public string Description;
            [JsonInclude] public int Price;
            [JsonInclude] public string Icon; // sprite guid 
        
            //dev function use only
            public ItemBaseData(int ID, string Name, ItemConstants.ItemCategory Category, string Description, string Icon)
            {
                this.ID = ID;
                this.Name = Name;
                this.Category = Category;
                this.Description = Description;
                this.Icon = Icon;
            }
        }
        
        public struct ItemData
        {
            public int ID; // ID must > 0
            public ItemConstants.ItemCategory Category;
            public string Name;
            public string Description;
            public int Price;
            public int Amount;
            public string Icon;

            public ItemData(ItemBaseData baseData, int amount)
            {
                ID = baseData.ID;
                Category = baseData.Category;
                Name = baseData.Name;
                Description = baseData.Description;
                Price = baseData.Price;
                Amount = amount;
                Icon = baseData.Icon;
            }

            public void Add(int amount) => Amount += amount;
        }

        public struct ItemUploadData
        {
            public int ID;
            public ItemConstants.ItemType ItemType;
            public int Amount;

            public ItemUploadData(ItemConstants.ItemType type, int id, int amount)
            {
                ID = id;
                ItemType = type;
                Amount = amount;
            }
        }
    }
}
