using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "character", menuName = "Scriptable Objects/character")]
public class CharacterSO : ScriptableObject
{
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public GameObject Character { get; private set; }
    [field: SerializeField] public List<Mesh> ExtraSkin { get; private set; }
    [field: SerializeField] public List<RuntimeAnimatorController> ExtraAnimation { get; private set; }
    [field: SerializeField, SerializedDictionary("material index","material variant")] public SerializedDictionary<int, Material> MaterialVariations { get; private set; }
}