using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Constant;
using UnityEngine;

[CreateAssetMenu(fileName = "CropSO", menuName = "Scriptable Objects/CropSO")]
public class CropSO : ScriptableObject
{
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public ItemConstants.Classification Classification { get; private set; }
    [field: SerializeField] public int HarvestCount { get; private set; }
    [field: SerializeField] public int Price { get; private set; }
    [field: SerializeField] public int Exp { get; private set; }
    [field: SerializeField] public int MatureTime { get; private set; } //in seconds
    [field: SerializeField, SerializedDictionary("CropStatus", "Mesh")] public SerializedDictionary<CropConstants.CropStatus, Mesh> Appearance { get; private set; } = new(2); //crucial
    [field: SerializeField, SerializedDictionary("CropStatus", "Materials")] public SerializedDictionary<CropConstants.CropStatus, Material[]> Material { get; private set; } = new(2); //optional
}