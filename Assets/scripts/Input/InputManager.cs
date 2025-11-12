using System;
using System.Collections.Generic;
using UnityEngine;
using Constant;
using Extensions;
using Unity.Cinemachine;
using UnityEngine.InputSystem.EnhancedTouch;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    
    [field: SerializeField] public GameObject CinemachinePrefab { get; private set; }

    public PlayerInput playerInput { get; private set; }
    public CinemachineCamera SpawnedCamera { get; set; }

    private void Awake()
    {
        Instance = this;

        EnhancedTouchSupport.Enable();
        
        playerInput = new PlayerInput();
        playerInput.Enable();

        playerInput.UI.Click.performed += this.GenerateRay; //generate raycast each time when clicking screen
    }

    public void EnableControl()
    {
        GameObject obj = UIManager.Instance.SpawnUI(UIConstants.NonPooledUITypes.ControlOverlay);
        UIManager.Instance.AllActiveUIs.Add(UIConstants.NonPooledUITypes.ControlOverlay, obj);
        
        Canvas inputCanvas = obj.GetComponent<Canvas>();
        inputCanvas.worldCamera = Camera.main;
        inputCanvas.planeDistance = 1000;
    }

    public void DisableControl()
    {
        Destroy(UIManager.Instance.AllActiveUIs[UIConstants.NonPooledUITypes.ControlOverlay]);
        UIManager.Instance.AllActiveUIs.Remove(UIConstants.NonPooledUITypes.ControlOverlay);
    }
    
    private void OnDestroy()
    {
        EnhancedTouchSupport.Disable();
        
        playerInput.Disable();
        playerInput.Dispose();
    }
}
