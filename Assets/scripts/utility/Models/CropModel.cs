using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Constant;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Models
{
    public static class CropModel
    {
        public struct CropData
        {
            // basic data
            public int ID;
            public string Name;
            public ItemConstants.ItemCategory Category;
            public string Description;
            public Dictionary<ItemConstants.ResourceType, int> Costs;
            public Dictionary<ItemConstants.ResourceType, int> Rewards;
            public Dictionary<CropConstants.CropStatus, Mesh> Appearance;
            public Dictionary<CropConstants.CropStatus, Material[]> Material;
            
            // standalone indication
            public CropConstants.CropStatus Status;
            public DateTimeOffset MatureTime;
            
            public CropData(CropSO baseData, DateTimeOffset matureTime, CropConstants.CropStatus status = CropConstants.CropStatus.Growing)
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

            public CropData(CropConstants.CropStatus status)
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