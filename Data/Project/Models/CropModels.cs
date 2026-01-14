using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unity.Services.CloudSave.Model;
using static Data.ItemConstants;
using static Data.CropConstants;

namespace Data;

public static class CropModels
{
    public class CropBaseData
    {
        // general data
        [JsonInclude] public int ID;
        [JsonInclude] public string Name;
        [JsonInclude] public ItemCategory Category;
        [JsonInclude] public string Description;
        [JsonInclude] public int TimeNeeded; // in seconds
        [JsonInclude] public Dictionary<ResourceType, int> Costs; //time will be in seconds 
        [JsonInclude] public Dictionary<ResourceType, int> Rewards;
        [JsonInclude] public Dictionary<CropStatus, string> Appearance; // mesh guid as string
        [JsonInclude] public Dictionary<CropStatus, string[]> Material; // material guids as string array

        //PlantCropUI use only
        [JsonInclude] public string Icon; // sprite guid 
        [JsonInclude] public string DetailBg; // sprite guid 
        [JsonInclude] public string DetailImage; // sprite guid

        //dev function use only
        public CropBaseData(int ID, string Name, ItemCategory Category, string Description, int TimeNeeded, Dictionary<ResourceType, int> Costs, Dictionary<ResourceType, int> Rewards, Dictionary<CropStatus, string> Appearance, Dictionary<CropStatus, string[]> Material, string Icon, string DetailBg, string DetailImage)
        {
            this.ID = ID;
            this.Name = Name;
            this.Category = Category;
            this.Description = Description;
            this.TimeNeeded = TimeNeeded;
            this.Costs = Costs;
            this.Rewards = Rewards;
            this.Appearance = Appearance;
            this.Material = Material;

            this.Icon = Icon;
            this.DetailBg = DetailBg;
            this.DetailImage = DetailImage;
        }
    }
    
    public class CropUploadData
    {
        [JsonInclude] public int CropID;
        [JsonInclude] public DateTimeOffset MatureTime; //in epoch timestamp , standalone indication

        public CropUploadData(int cropID, DateTimeOffset matureTime) //constructor
        {
            CropID = cropID;
            MatureTime = matureTime;
        }
    }
}