using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Unity.Netcode;
using UnityEngine;

public enum ObjectPoolType
{
    test
}

public class PoolManager : NetworkBehaviour
{
    [field: SerializeField, SerializedDictionary("material index", "material variant")]
    public SerializedDictionary<ObjectPoolType, SingleNetworkObjectPool> AllObjectPools { get; private set; }
    
    private List<ObjectPoolType> RegisteredPool = new();
    
    public static PoolManager Instance;

    private void Awake()
    {
        //setup singleton
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    //register all pool to NetworkPrefabInstanceHandler
    public override void OnNetworkSpawn()
    {
        foreach (KeyValuePair<ObjectPoolType, SingleNetworkObjectPool> element in AllObjectPools)
        {
            GameObject prefab = AllObjectPools[element.Key].Prefab; 
        
            RegisteredPool.Add(element.Key);
            NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new PooledPrefabInstanceHandler(element.Key));
        }
    }
    
    public override void OnNetworkDespawn()
    {
        //unregister all objects in all pools (by source prefab) and clean up the pools
        foreach (ObjectPoolType element in RegisteredPool)
        {
            NetworkManager.Singleton.PrefabHandler.RemoveHandler(AllObjectPools[element].Prefab);
            AllObjectPools[element].Instance.Clear();
        }
        
        RegisteredPool.Clear();
    }
    
    //wrapper get/release
    public NetworkObject Get(ObjectPoolType poolType, Vector3 position, Quaternion rotation)
    {
        NetworkObject objNO = AllObjectPools[poolType].Instance.Get();

        objNO.transform.position = position;
        objNO.transform.rotation = rotation;
        return objNO;
    }
    
    public void Release(ObjectPoolType poolType, NetworkObject obj) => AllObjectPools[poolType].Instance.Release(obj);
}

public class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
{
    private ObjectPoolType poolType;

    public PooledPrefabInstanceHandler(ObjectPoolType poolType) => this.poolType = poolType;

    NetworkObject INetworkPrefabInstanceHandler.Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation) =>
        PoolManager.Instance.Get(poolType, position, rotation);

    void INetworkPrefabInstanceHandler.Destroy(NetworkObject networkObject) =>
        PoolManager.Instance.Release(poolType, networkObject);
}