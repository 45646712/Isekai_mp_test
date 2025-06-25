using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class SingleNetworkObjectPool : NetworkBehaviour
{
    [SerializeField] private PoolObjectSO poolData;

    private IObjectPool<NetworkObject> constructedInstance;
    
    private bool checkPool = true;

    //SO data
    public GameObject Prefab { get; private set; }
    private int DefaultPoolSize;
    private int MaxPoolSize;
    
    public IObjectPool<NetworkObject> Instance
    {
        get
        {
            if (constructedInstance == null)
            {
                constructedInstance = new ObjectPool<NetworkObject>(CreatePoolObj, OnGetPoolObj, OnReturnPoolObj, OnDestroyPoolObj, checkPool, DefaultPoolSize, MaxPoolSize);
            }

            return constructedInstance;
        }
    }

    private void Awake()
    {
        //setup pool
        Prefab = poolData.Prefab;
        DefaultPoolSize = poolData.DefaultPoolSize;
        MaxPoolSize = poolData.MaxPoolSize;
    }

    private NetworkObject CreatePoolObj()
    {
        GameObject poolObj = Instantiate(Prefab);
        
        if (IsServer)
        {
            poolObj.GetComponent<NetworkObject>().Spawn();
        }
        
        if (!IsServer && poolObj.GetComponent<NetworkRigidbody>() == null)
        {
            poolObj.AddComponent<NetworkRigidbody>();
        }
        
        return poolObj.GetComponent<NetworkObject>();
    }

    public void OnGetPoolObj(NetworkObject poolObj) => poolObj.gameObject.SetActive(true);
    public void OnReturnPoolObj(NetworkObject poolObj) => poolObj.gameObject.SetActive(false);
    
    private void OnDestroyPoolObj(NetworkObject poolObj)
    {
        if (IsServer)
        {
            poolObj.Despawn();
        }
    }
}