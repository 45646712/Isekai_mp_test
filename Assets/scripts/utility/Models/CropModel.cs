using System;
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
            public int ID;
            public ItemConstants.Classification Classification;
            public CropConstants.CropStatus Status;
            public int HarvestCount;
            public int Price;
            public int Exp;
            public DateTimeOffset MatureTime;
            public SerializedDictionary<CropConstants.CropStatus, Mesh> Appearance;
            public SerializedDictionary<CropConstants.CropStatus, Material[]> Material;

            public CropData(CropSO baseData, DateTimeOffset matureTime, CropConstants.CropStatus status = CropConstants.CropStatus.Growing)
            {
                ID = baseData.ID;
                Classification = baseData.Classification;
                Status = status;
                HarvestCount = baseData.HarvestCount;
                Price = baseData.Price;
                Exp = baseData.Exp;
                MatureTime = matureTime;
                Appearance = baseData.Appearance;
                Material = baseData.Material;
            }

            public void Reset()
            {
                this = default;
                Status = CropConstants.CropStatus.Null;
            }
        }

        public struct CropUploadData
        {
            public int SlotID;
            public int CropID;
            public DateTimeOffset MatureTime; //in epoch timestamp

            public CropUploadData(int slotID, int cropID, DateTimeOffset matureTime) //constructor
            {
                SlotID = slotID;
                CropID = cropID;
                MatureTime = matureTime;
            }
        }
    }
}