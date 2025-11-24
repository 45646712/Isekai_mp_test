using System;
using System.Collections.Generic;
using System.Text.Json;
using Newtonsoft.Json;
using Unity.Services.CloudSave.Model;

using static Data.CropConstants;
using static Data.ItemConstants;

namespace Data;

public class CropModels
{
    public struct Crop // game data
    {
        // general data
        public int ID;
        public string Name;
        public ItemCategory Category;
        public string Description;
        public Dictionary<ResourceType, int> Costs; //time will be in seconds 
        public Dictionary<ResourceType, int> Rewards;
        public Dictionary<CropStatus, string> Appearance; // mesh guid as string
        public Dictionary<CropStatus, string[]> Material; // material guids as string array

        //PlantCropUI use only
        public string Icon; // sprite guid 
        public string DetailBg; // sprite guid 
        public string DetailImage; // sprite guid 

        public Crop(string data)
        {
            this = JsonConvert.DeserializeObject<Crop>(data);
        }
        
        public SetItemBatchBody ConvertToGameData()
        {
            return new SetItemBatchBody(new()
            {
                new SetItemBody(nameof(CropGameData.ID), ID),
                new SetItemBody(nameof(CropGameData.Name), Name),
                new SetItemBody(nameof(CropGameData.Category), Category),
                new SetItemBody(nameof(CropGameData.Description), Description),
                new SetItemBody(nameof(CropGameData.Costs), Costs),
                new SetItemBody(nameof(CropGameData.Rewards), Rewards),
                new SetItemBody(nameof(CropGameData.Appearance), Appearance),
                new SetItemBody(nameof(CropGameData.Material), Material),
                new SetItemBody(nameof(CropGameData.Icon), Icon),
                new SetItemBody(nameof(CropGameData.DetailBg), DetailBg),
                new SetItemBody(nameof(CropGameData.DetailImage), DetailImage)
            });
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