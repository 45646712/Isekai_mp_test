using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_movement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
        
    private PlayerInput playerInput;
    private InputActionAsset playerAction;

    private InputAction move;
    
    private Vector3 moveValue; 
    NetworkVariable<string> test = new NetworkVariable<string>("1",NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
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
    }

    private void Update()
    {
        if (playerInput != null)
        {
            moveValue = new Vector3(move.ReadValue<Vector2>().x, 0, move.ReadValue<Vector2>().y);
            transform.position += moveValue * moveSpeed * Time.deltaTime;
        }
    }
}
