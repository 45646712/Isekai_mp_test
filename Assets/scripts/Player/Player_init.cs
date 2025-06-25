using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_init : NetworkBehaviour
{
    [SerializeField] private CharacterSO data;

    private Renderer meshRenderer;
    
    protected override void OnNetworkPostSpawn()
    {
        if (!IsOwner)
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

        GetComponent<PlayerInput>().enabled = IsOwner;
    }
}