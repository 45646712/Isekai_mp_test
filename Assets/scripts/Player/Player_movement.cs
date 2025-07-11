using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_movement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;

    private PlayerInput playerInput;
    private InputActionAsset playerAction;

    private InputAction move;
    private InputAction spawnTest;

    private Vector3 moveValue;

    private NetworkVariable<Vector3> posData = new(Vector3.zero);

    protected override void OnNetworkPostSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        playerInput = GetComponent<PlayerInput>();

        playerInput.enabled = true;
        playerAction = playerInput.actions;

        move = playerAction.FindAction("Move");
        spawnTest = playerAction.FindAction("Interact");

        spawnTest.started += OnTestSpawn;
        //posData = new NetworkVariable<Vector3>(transform.position, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Owner);
    }

    private void Update()
    {
        if (playerInput != null)
        {
            Vector3 tempPos = posData.Value;
            moveValue = new Vector3(move.ReadValue<Vector2>().x, 0, move.ReadValue<Vector2>().y);
            tempPos += moveValue * moveSpeed * Time.deltaTime;

            //posData = new NetworkVariable<Vector3>(tempPos, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Owner);
        }
    }

    public void OnTestSpawn(InputAction.CallbackContext context)
    {
        if (IsServer && IsSpawned)
        {
            OnTestCallback(context.started);
        }
        else if (!IsServer && IsSpawned)
        {
            OnTestSpawnServerRpc(context.started);
        }
    }

    [ServerRpc]
    public void OnTestSpawnServerRpc(bool contextState) => OnTestCallback(contextState);

    public void OnTestCallback(bool contextState)
    {
        PoolManager.Instance.Get(ObjectPoolType.test, new Vector3(0, 5, 0), Quaternion.identity);
    }
}