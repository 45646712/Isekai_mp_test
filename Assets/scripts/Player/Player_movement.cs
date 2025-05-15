using System;
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
}
