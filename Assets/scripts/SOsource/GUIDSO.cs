using System;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GUIDSO", menuName = "Scriptable Objects/GUIDSO")]
public class GUIDSO : ScriptableObject
{
    [SerializeField] private Asset
    [SerializeField] private string guidString = Guid.NewGuid().ToString();

    [NonSerialized]
    private Guid guid = Guid.Empty;

    public Guid Guid
    {
        get
        {
            if (guid == Guid.Empty)
            {
                guid = new Guid(guidString);
            }
       
            return guid;
        }       
    }
}
