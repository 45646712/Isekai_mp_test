using UnityEngine;

[CreateAssetMenu(fileName = "characterSO", menuName = "Scriptable Objects/characterSO")]
public class CharacterSO : ScriptableObject
{
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public Mesh CharacterMesh { get; private set; }
    [field: SerializeField] public Material CharacterMaterial { get; private set; }
    [field: SerializeField] public RuntimeAnimatorController Animator { get; private set; }
}
