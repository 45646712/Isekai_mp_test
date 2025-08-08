using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInput : InputAxisControllerBase<Reader>
{
    [Range(0, 20)] public float LookSpeed = 6;
    
    private void Start()
    {
        InputManager.Instance.playerInput.Player.Look.started += value =>
        {
            foreach (Controller element in Controllers)
            {
                element.Input.ProcessInput(value.action, LookSpeed);
            }
        };
    }
    
    void Update()
    {
        if (Application.isPlaying)
        {
            UpdateControllers(); //intrinsic input update
            
            foreach (Controller element in Controllers)
            {
                element.Input.Reset();
            }
        }
    }
}

[Serializable]
public class Reader : IInputAxisReader
{
    Vector2 value; // the cached value of the input

    public void Reset() => value = Vector2.zero;
    public void ProcessInput(InputAction action, float speed) => value = action.ReadValue<Vector2>() * speed;
    public float GetValue(UnityEngine.Object context, IInputAxisOwner.AxisDescriptor.Hints hint) => hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? value.y : value.x;
}