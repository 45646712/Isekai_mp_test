using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player_init : NetworkBehaviour
{
    [SerializeField] private CharacterSO data;

    private Renderer meshRenderer;
    
    public override void OnNetworkSpawn()
    {
        GameObject character = Instantiate(data.Character, gameObject.transform);

        if (!IsOwner)
        {
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
            NetworkManager.Singleton.SceneManager.OnLoadComplete += (v1, v2, v3) =>
            {
                GameObject cam = Instantiate(InputManager.Instance.CinemachinePrefab);
                CinemachineCamera camCore = cam.GetComponent<CinemachineCamera>();
                
                camCore.Follow = gameObject.transform;
                cam.GetComponent<CameraInput>().enabled = true;

                InputManager.Instance.SpawnedCamera = camCore;
            };

            if (InputManager.Instance.SpawnedCamera != null)
            {
                InputManager.Instance.SpawnedCamera.Follow = gameObject.transform;
            }
        }
    }
}