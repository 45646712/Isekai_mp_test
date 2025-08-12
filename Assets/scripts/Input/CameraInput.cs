using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInput : InputAxisControllerBase<Reader>
{
    [Range(1, 5)] public float LookSpeed;
    [Range(1, 5), SerializeField] private float verticalRatio;
    [Range(1, 5), SerializeField] private float horizontalRatio;
    
    private void Start()
    {
        InputManager.Instance.playerInput.Player.Look.performed += context =>
        {
            foreach (Controller element in Controllers)
            {
                element.Input.ProcessInput(context.action, LookSpeed, verticalRatio, horizontalRatio);
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
    private Vector2 value; // the cached value of the input

    public void Reset() => value = Vector2.zero;

    public void ProcessInput(InputAction action, float speed, float verticalRatio, float horizontalRatio)
    {
        Vector2 inputval = action.ReadValue<Vector2>();
        value = new Vector2(inputval.x * horizontalRatio, inputval.y * verticalRatio) * speed * 60;
    }

    public float GetValue(UnityEngine.Object context, IInputAxisOwner.AxisDescriptor.Hints hint) => hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? value.y : value.x;
}