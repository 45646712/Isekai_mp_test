using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "characterSO", menuName = "Scriptable Objects/characterSO")]
public class CharacterSO : ScriptableObject
{
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public GameObject Character { get; private set; }
    [field: SerializeField] public List<Mesh> ExtraSkin { get; private set; }
    [field: SerializeField] public List<RuntimeAnimatorController> ExtraAnimation { get; private set; }
    [field: SerializeField] public List<Material> OtherMaterial { get; private set; }
}
