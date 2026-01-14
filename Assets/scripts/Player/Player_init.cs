using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Player_init : NetworkBehaviour
{
    [SerializeField] private CharacterSO data;

    private Renderer meshRenderer;
    
    public override void OnNetworkSpawn()
    {
        GameObject character = Instantiate(data.Character, gameObject.transform);

        if (OwnerClientId != NetworkManager.Singleton.LocalClientId)
        {
            meshRenderer = character.GetComponentInChildren<SkinnedMeshRenderer>();
            List<Material> materials = meshRenderer.materials.ToList();

            foreach (KeyValuePair<int, Material> element in data.MaterialVariations)
            {
                materials[element.Key] = element.Value;
            }

            meshRenderer.materials = materials.ToArray();

            GetComponentInChildren<Animator>().runtimeAnimatorController = data.ExtraAnimation[0]; //differentiate host/client
        }
        else
        {
            SpawnManager.Instance.player = NetworkObject;
            
            SpawnManager.Instance.spawnedCamera.Follow = gameObject.transform;
            SpawnManager.Instance.spawnedCamera.GetComponent<CameraInput>().enabled = true;

            if (SpawnManager.Instance.spawnedCamera)
            {
                SpawnManager.Instance.spawnedCamera.Follow = gameObject.transform;
            }
        }
    }
}