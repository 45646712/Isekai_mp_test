using System;
using AYellowpaper.SerializedCollections;
using Constant;
using UnityEngine;

public class NonNetworkEntityParent : MonoBehaviour
{
    public static NonNetworkEntityParent instance;

    [field: SerializeField, SerializedDictionary("EntityType", "EntityParent")] public SerializedDictionary<UIConstants.NonNetworkEntityTypes, Transform> EntityParents { get; private set; }

    private void Awake()
    {
        instance = this;
    }
}
