using UnityEngine;
using UnityEngine.Pool;

public class SingleObjectPool : MonoBehaviour
{
    [SerializeField] private PoolObjectSO poolData;
    
    private IObjectPool<GameObject> constructedInstance;
    
    private bool checkPool = true;
    
    //SO data
    public GameObject Prefab { get; private set; }
    private int DefaultPoolSize;
    private int MaxPoolSize;
    
    public IObjectPool<GameObject> Instance
    {
        get
        {
            if (constructedInstance == null)
            {
                constructedInstance = new ObjectPool<GameObject>(CreatePoolObj, OnGetPoolObj, OnReturnPoolObj, OnDestroyPoolObj, checkPool, DefaultPoolSize, MaxPoolSize);
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
    
    private GameObject CreatePoolObj() => Instantiate(Prefab);
    private void OnGetPoolObj(GameObject poolObj) => poolObj.SetActive(true);
    private void OnReturnPoolObj(GameObject poolObj) => poolObj.SetActive(false);
    private void OnDestroyPoolObj(GameObject poolObj) => Destroy(poolObj); //same as return(release) all
}