using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using UnityEngine;
using Constant;

using static Constant.ItemConstants;
using static Constant.CropConstants;

namespace Models
{
    public static class CropModel
    {
        public class CropBaseData
        {
            // general data
            [JsonInclude] public int ID;
            [JsonInclude] public string Name;
            [JsonInclude] public ItemCategory Category;
            [JsonInclude] public string Description;
            [JsonInclude] public int TimeNeeded; // in seconds
            [JsonInclude] public Dictionary<ResourceType, int> Costs;
            [JsonInclude] public Dictionary<ResourceType, int> Rewards;
            [JsonInclude] public Dictionary<CropStatus, string> Appearance; // mesh guid as string
            [JsonInclude] public Dictionary<CropStatus, string[]> Material; // material guids as string array

            //PlantCropUI use only
            [JsonInclude] public string Icon; // sprite guid 
            [JsonInclude] public string DetailBg; // sprite guid 
            [JsonInclude] public string DetailImage; // sprite guid
        }

        public class CropData
        {
            public int ID { get; private set; }
            public string Name { get; private set; }
            public ItemCategory Category { get; private set; }
            public string Description { get; private set; }
            public int TimeNeeded { get; private set; }
            public Dictionary<CropStatus, Mesh> Appearance { get; private set; }
            public Dictionary<CropStatus, Material[]> Material { get; private set; }
            
            public Sprite Icon{ get; private set; }
            public Sprite DetailBg{ get; private set; }
            public Sprite DetailImage{ get; private set; }

            public CropStatus Status { get; private set; }
            public DateTimeOffset MatureTime { get; private set; } //execute as usual , validate on cloudcode

            public CropData(CropBaseData baseData, DateTimeOffset matureTime, CropStatus status = CropStatus.Growing)
            {
                AssetSO allMesh = AssetManager.Instance.AllAssets[AssetConstants.AssetType.Mesh];
                AssetSO allMaterial = AssetManager.Instance.AllAssets[AssetConstants.AssetType.Material];

                ID = baseData.ID;
                Name = baseData.Name;
                Category = baseData.Category;
                Description = baseData.Description;
                TimeNeeded = baseData.TimeNeeded;
                Appearance = baseData.Appearance.ToDictionary(x => x.Key, x => (Mesh)allMesh.GetAsset(x.Value));
                Material = baseData.Material.ToDictionary(x => x.Key, x => x.Value.Select(y => (Material)allMaterial.GetAsset(y)).ToArray());

                Icon = (Sprite)AssetManager.Instance.AllAssets[AssetConstants.AssetType.Sprite].GetAsset(baseData.Icon);
                DetailBg = (Sprite)AssetManager.Instance.AllAssets[AssetConstants.AssetType.Sprite].GetAsset(baseData.DetailBg);
                DetailImage = (Sprite)AssetManager.Instance.AllAssets[AssetConstants.AssetType.Sprite].GetAsset(baseData.DetailImage);
                
                Status = status;
                MatureTime = matureTime;
            }

            public CropData() => Status = CropStatus.Null;

            public void Lock() => Status = CropStatus.Locked;
            public void UnLock() => Status = CropStatus.Null;
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
}