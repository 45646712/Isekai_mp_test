using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Constant;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "AssetSO", menuName = "Scriptable Objects/AssetSO")]
public class AssetSO : ScriptableObject
{
    [Serializable]
    private struct AssetRef
    {
        public string GUID;
        public Object Asset;

        public AssetRef(string GUID, Object Asset)
        {
            this.GUID = GUID;
            this.Asset = Asset;
        }
    }

    [field: SerializeField] private AssetRef[] source { get; set; }
    
    public void Check() => source = source.Select(x => string.IsNullOrEmpty(x.GUID) ? new AssetRef(Guid.NewGuid().ToString(), x.Asset) : x).ToArray();

    public Object GetAsset(string GUID) => source.First(x => x.GUID == GUID).Asset;
}