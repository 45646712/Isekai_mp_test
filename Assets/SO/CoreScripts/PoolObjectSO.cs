using UnityEngine;

[CreateAssetMenu(fileName = "PoolObject", menuName = "Scriptable Objects/PoolObject")]
public class PoolObjectSO : ScriptableObject
{
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public int DefaultPoolSize { get; private set; }
    [field: SerializeField] public int MaxPoolSize { get; private set; }
}