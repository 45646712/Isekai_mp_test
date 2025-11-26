using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;
using Constant;

using static Constant.ItemConstants;
using static Constant.CropConstants;

namespace Models
{
    public static class CropModel
    {
        public struct Crop
        {
            // general data
            [JsonInclude] public int ID;
            [JsonInclude] public string Name;
            [JsonInclude] public ItemCategory Category;
            [JsonInclude] public string Description;
            [JsonInclude] public Dictionary<ResourceType, int> Costs; //time will be in seconds 
            [JsonInclude] public Dictionary<ResourceType, int> Rewards;
            [JsonInclude] public Dictionary<CropStatus, string> Appearance; // mesh guid as string
            [JsonInclude] public Dictionary<CropStatus, string[]> Material; // material guids as string array

            //PlantCropUI use only
            [JsonInclude] public string Icon; // sprite guid 
            [JsonInclude] public string DetailBg; // sprite guid 
            [JsonInclude] public string DetailImage; // sprite guid
        }

        public struct CropData
        {
            // basic data
            public int ID;
            public string Name;
            public ItemCategory Category;
            public string Description;
            public Dictionary<ResourceType, int> Costs;
            public Dictionary<ResourceType, int> Rewards;
            public Dictionary<CropStatus, Mesh> Appearance;
            public Dictionary<CropStatus, Material[]> Material;
            
            // standalone indication
            public CropStatus Status;
            public DateTimeOffset MatureTime;
            
            public CropData(CropSO baseData, DateTimeOffset matureTime, CropStatus status = CropStatus.Growing)
            {
                ID = baseData.ID;
                Name = baseData.Name;
                Category = baseData.Category;
                Description = baseData.Description;
                Costs = baseData.Costs;
                Rewards = baseData.Rewards;
                Appearance = baseData.Appearance;
                Material = baseData.Material;

                Status = status;
                MatureTime = matureTime;
            }

            public CropData(CropStatus status)
            {
                this = default;
                Status = status;
            }
        }

        public struct CropUploadData
        {
            public int SlotID;
            public int CropID;
            public DateTimeOffset MatureTime; //in epoch timestamp , standalone indication

            public CropUploadData(int slotID, int cropID, DateTimeOffset matureTime) //constructor
            {
                SlotID = slotID;
                CropID = cropID;
                MatureTime = matureTime;
            }
        }
    }
}