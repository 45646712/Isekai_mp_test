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
        if (!IsOwnedByServer)
        {
            GameObject character = Instantiate(data.Character, gameObject.transform);
            
            meshRenderer = character.GetComponentInChildren<SkinnedMeshRenderer>();
            List<Material> materials = meshRenderer.materials.ToList();
            
            foreach (KeyValuePair<int, Material> element in data.MaterialVariations)
            {
                materials[element.Key] = element.Value;
            }
        
            meshRenderer.materials = materials.ToArray();

            GetComponentInChildren<Animator>().runtimeAnimatorController = data.ExtraAnimation[0];
        }
        else
        {
            Instantiate(data.Character, gameObject.transform);
        }
    }
}
