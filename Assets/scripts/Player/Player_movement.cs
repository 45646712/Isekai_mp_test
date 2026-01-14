using System;
using Unity.Netcode;
using UnityEngine;

public class Player_movement : NetworkBehaviour
{
    [Range(2, 10), SerializeField] private float moveSpeed;
    [Range(360, 720), SerializeField] private float lookSpeed;

    private Vector2 input;
    private Rigidbody rb;

    public override void OnNetworkSpawn()
    {
        enabled = OwnerClientId == NetworkManager.Singleton.LocalClientId;
        
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        input = InputManager.Instance.playerInput.Player.Move.ReadValue<Vector2>();

        Vector3 cameraDirection = Camera.main.transform.TransformDirection(new Vector3(input.x, 0, input.y));
        cameraDirection.y = 0;
        cameraDirection.Normalize();

        MoveRpc(cameraDirection * moveSpeed);
    }

    [Rpc(SendTo.Server)]
    private void MoveRpc(Vector3 displacement)
    {
        rb.linearVelocity = displacement;

        if (displacement != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(displacement, Vector3.up), lookSpeed * Time.deltaTime);
        }
    }
}