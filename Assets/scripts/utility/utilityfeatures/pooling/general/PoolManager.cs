using AYellowpaper.SerializedCollections;
using UnityEngine;

public enum ObjectPoolType
{
    SessionDetail,
    RequestDetail,
    CropIcon,
    CropCostIcon
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    
    [field: SerializeField, SerializedDictionary("Pool Types", "Pool")] 
    public SerializedDictionary<ObjectPoolType, SingleObjectPool> AllObjectPools { get; private set; }
    
    private void Awake()
    {
        //setup singleton
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    //wrapper objectpool functions
    public GameObject Get(ObjectPoolType poolType, Transform parent)
    {
        GameObject obj = AllObjectPools[poolType].Instance.Get();
        obj.transform.SetParent(parent,false);
        
        return obj;
    }
    
    public void Release(ObjectPoolType poolType, GameObject obj) => AllObjectPools[poolType].Instance.Release(obj);
    public void Clear(ObjectPoolType poolType) => AllObjectPools[poolType].Instance.Clear();
}