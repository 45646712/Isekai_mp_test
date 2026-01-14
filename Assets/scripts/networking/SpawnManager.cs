using System;
using Constant;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation
public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager Instance;

    [field: SerializeField] private NetworkObject PlayerPrefab;
    [field: SerializeField] private GameObject CinemachinePrefab;

    public CinemachineCamera spawnedCamera { get; private set; }
    public NetworkObject player { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        spawnedCamera = Instantiate(CinemachinePrefab).GetComponent<CinemachineCamera>();
        
        SpawnPlayerRpc(NetworkManager.Singleton.LocalClientId);
        
        InputManager.Instance.EnableControl();
    }

    [Rpc(SendTo.Server)] // client/host -> server : spawn client a player object
    private void SpawnPlayerRpc(ulong clientId) => Instantiate(PlayerPrefab).SpawnAsPlayerObject(clientId);

    public override void OnNetworkDespawn()
    {
        if (player is { IsSpawned: true })
        {
            player.Despawn();
        }

        if (spawnedCamera)
        {
            Destroy(spawnedCamera.gameObject);
        }
        
        if (UIManager.Instance.AllActiveUIs.ContainsKey(UIConstants.NonPooledUITypes.ControlOverlay))
        {
            InputManager.Instance.DisableControl();
        }
    }
}
