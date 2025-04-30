using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private CharacterSO OwnerSO;
    [SerializeField] private CharacterSO NotOwnerSO;
    
    private Mesh CharacterMesh;
    private Material CharacterMaterial;
    private RuntimeAnimatorController Animator;
    
    protected override void OnNetworkPostSpawn()
    {
        if (IsOwner)
        {
            GetComponent<MeshFilter>().mesh = OwnerSO.CharacterMesh;
            GetComponent<MeshRenderer>().material = OwnerSO.CharacterMaterial;
            GetComponent<Animator>().runtimeAnimatorController = OwnerSO.Animator;
        }
        else
        {
            GetComponent<MeshFilter>().mesh = NotOwnerSO.CharacterMesh;
            GetComponent<MeshRenderer>().material = NotOwnerSO.CharacterMaterial;
            GetComponent<Animator>().runtimeAnimatorController = NotOwnerSO.Animator;
        } 
    }
}
