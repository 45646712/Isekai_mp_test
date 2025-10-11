using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Constant;
using Models;
using UnityEngine;

[CreateAssetMenu(fileName = "CropSO", menuName = "Scriptable Objects/CropSO")]
public class CropSO : ScriptableObject
{
    [field: Header("Data"), Space(10)]
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public ItemConstants.ItemCategory Category { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField, SerializedDictionary("ResourceType", "Value")] public SerializedDictionary<ItemConstants.ResourceType, int> Costs { get; private set; } //time will be in seconds
    [field: SerializeField, SerializedDictionary("ResourceType", "Value")] public SerializedDictionary<ItemConstants.ResourceType, int> Rewards { get; private set; }
    [field: SerializeField, SerializedDictionary("CropStatus", "Mesh")] public SerializedDictionary<CropConstants.CropStatus, Mesh> Appearance { get; private set; } = new(2); //crucial
    [field: SerializeField, SerializedDictionary("CropStatus", "Materials")] public SerializedDictionary<CropConstants.CropStatus, Material[]> Material { get; private set; } = new(2); //optional

    [field: Header("PlantCropUI use only"), Space(10)]
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Sprite DetailBg { get; private set; }
    [field: SerializeField] public Sprite DetailImage { get; private set; }
}