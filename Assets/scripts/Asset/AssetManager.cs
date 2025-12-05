using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Constant;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetManager : MonoBehaviour
{
    public static AssetManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [field: SerializeField, SerializedDictionary("AssetType", "AssetInfo")] public SerializedDictionary<AssetConstants.AssetType, AssetSO> AllAssets { get; private set; }
}
