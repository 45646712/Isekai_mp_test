using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerInit : NetworkBehaviour
{
    [SerializeField] private CharacterSO data;

    private Renderer meshRenderer;

    protected override void OnNetworkPostSpawn()
    {
        GameObject character;
        if (!IsOwnedByServer)
        {
            character = Instantiate(data.Character, gameObject.transform);
            transform.position = new Vector3(2, 2, 0);
        }
        else
        {
            character = Instantiate(data.Character, gameObject.transform);
        }



        meshRenderer = character.GetComponentInChildren<SkinnedMeshRenderer>();
        List<Material> materials = meshRenderer.materials.ToList();
        
        if (!IsOwnedByServer)
        {
            materials[0] = data.OtherMaterial[0];
            materials[1] = data.OtherMaterial[0];
        
            meshRenderer.materials = materials.ToArray();

            GetComponentInChildren<Animator>().runtimeAnimatorController = data.ExtraAnimation[0];
        }
    }
}
