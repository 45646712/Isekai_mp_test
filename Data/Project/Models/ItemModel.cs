using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using static Data.ItemConstants;

namespace Data;

public class ItemModel
{
    public class ItemBaseData
    {
        // general data
        [JsonInclude] public int ID;
        [JsonInclude] public string Name;
        [JsonInclude] public ItemCategory Category;
        [JsonInclude] public string Description;
        [JsonInclude] public int Price;
        [JsonInclude] public string Icon; // sprite guid 
        
        //dev function use only
        public ItemBaseData(int ID, string Name, ItemCategory Category, string Description, string Icon)
        {
            this.ID = ID;
            this.Name = Name;
            this.Category = Category;
            this.Description = Description;
            this.Icon = Icon;
        }
    }
}