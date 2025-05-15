using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public enum ObjectPoolType
    {
        test
    }

    [field: SerializeField, SerializedDictionary("material index", "material variant")]
    public SerializedDictionary<ObjectPoolType, SingleObjectPool> AllObjectPools { get; private set; }
    
    public static PoolManager Instance;

    private void Awake()
    {
        //setup singleton
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
