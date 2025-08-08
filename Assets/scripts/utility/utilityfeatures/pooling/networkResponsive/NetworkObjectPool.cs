using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Unity.Netcode;
using UnityEngine;

public enum NetworkObjectPoolType
{
    test
}

public class NetworkPoolManager : NetworkBehaviour
{
    [field: SerializeField, SerializedDictionary("Pool Types", "Pool")]
    public SerializedDictionary<NetworkObjectPoolType, SingleNetworkObjectPool> AllNetworkObjectPools { get; private set; }
    
    private List<NetworkObjectPoolType> RegisteredPool = new();
    
    public static NetworkPoolManager Instance;

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
        foreach (var(key,value) in AllNetworkObjectPools)
        {
            GameObject prefab = AllNetworkObjectPools[key].Prefab; 
        
            RegisteredPool.Add(key);
            NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new PooledPrefabInstanceHandler(key));
        }
    }
    
    public override void OnNetworkDespawn()
    {
        //unregister all objects in all pools (by source prefab) and clean up the pools
        foreach (NetworkObjectPoolType element in RegisteredPool)
        {
            NetworkManager.Singleton.PrefabHandler.RemoveHandler(AllNetworkObjectPools[element].Prefab);
            AllNetworkObjectPools[element].Instance.Clear();
        }
        
        RegisteredPool.Clear();
    }
    
    //wrapper get/release
    public NetworkObject Get(NetworkObjectPoolType poolType, Vector3 position, Quaternion rotation)
    {
        NetworkObject objNO = AllNetworkObjectPools[poolType].Instance.Get();

        objNO.transform.position = position;
        objNO.transform.rotation = rotation;
        return objNO;
    }
    
    public void Release(NetworkObjectPoolType poolType, NetworkObject obj) => AllNetworkObjectPools[poolType].Instance.Release(obj);
}

public class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
{
    private NetworkObjectPoolType poolType { get; }

    public PooledPrefabInstanceHandler(NetworkObjectPoolType type)
    {
        poolType = type;
    }
    
    NetworkObject INetworkPrefabInstanceHandler.Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation) => NetworkPoolManager.Instance.Get(poolType, position, rotation);
    void INetworkPrefabInstanceHandler.Destroy(NetworkObject networkObject) => NetworkPoolManager.Instance.Release(poolType, networkObject);
}