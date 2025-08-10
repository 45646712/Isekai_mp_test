using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player_movement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;

    private Vector2 input;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        
        InputManager.Instance.EnableControl();
    }

    private void Update()
    {
        input = InputManager.Instance.playerInput.Player.Move.ReadValue<Vector2>();
        MoveRpc(new Vector3(input.x, 0, input.y) * moveSpeed);
    }
    
    [Rpc(SendTo.Server)]
    private void MoveRpc(Vector3 displacement)
    {
        rb.linearVelocity = displacement;
    }
}