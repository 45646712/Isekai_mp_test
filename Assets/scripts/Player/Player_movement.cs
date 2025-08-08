using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player_movement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;

    private Vector3 moveValue;
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
        InputManager.Instance.joystickCore.OnDragEvt += ProcessJoystickInput;
        InputManager.Instance.joystickCore.OnReleasedEvt += RecoverJoystickInput;
    }

    private void RecoverJoystickInput() => MoveRpc(Vector3.zero);
    
    private void ProcessJoystickInput()
    {
        Vector2 input = InputManager.Instance.joystickCore.Direction.normalized;
        
        MoveRpc(new Vector3(input.x, 0, input.y) * moveSpeed);
    }

    [Rpc(SendTo.Server)]
    private void MoveRpc(Vector3 displacement)
    {
        rb.linearVelocity = displacement;
    }
    
    public override void OnNetworkDespawn()
    {
        InputManager.Instance.joystickCore.OnDragEvt -= ProcessJoystickInput;
        InputManager.Instance.joystickCore.OnReleasedEvt -= RecoverJoystickInput;
    }
}