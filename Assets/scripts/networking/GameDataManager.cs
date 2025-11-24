using System;
using System.Collections.Generic;
using Constant;
using Cysharp.Threading.Tasks;
using Extensions;
using Newtonsoft.Json;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings.Data;
using Unity.Services.CloudSave;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    [SerializeField] private CropSO testso;

    private void Awake()
    {
        instance = this;
    }

    public async UniTask testcall()
    {
        await CloudCodeManager.Instance.SaveGameData(DataConstants_GameDataType.Crop, $"{nameof(DataConstants_GameDataType.Crop)}_{testso.ID}", JsonConvert.SerializeObject(new Crop(testso)));
    }
}

public struct Crop // game data
{
    // general data
    public int ID;
    public string Name;
    public ItemConstants.ItemCategory Category;
    public string Description;
    public Dictionary<ItemConstants.ResourceType, int> Costs; //time will be in seconds 
    public Dictionary<ItemConstants.ResourceType, int> Rewards;
    public Dictionary<CropConstants.CropStatus, string> Appearance; // mesh guid as string
    public Dictionary<CropConstants.CropStatus, string[]> Material; // material guids as string array

    //PlantCropUI use only
    public string Icon; // sprite guid 
    public string DetailBg; // sprite guid 
    public string DetailImage; // sprite guid 
    
    public Crop(CropSO baseData)
    {
        ID = baseData.ID;
        Name = baseData.Name;
        Category = baseData.Category;
        Description = baseData.Description;
        Costs = baseData.Costs;
        Rewards = baseData.Rewards;
        Appearance = new() { { CropConstants.CropStatus.Growing, "test" } };
        Material = new() { { CropConstants.CropStatus.Growing, new[] { "test1", "test2" } } };
        Icon = "1";
        DetailBg = "2";
        DetailImage = "3";
    }
}