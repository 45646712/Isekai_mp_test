using System;
using System.Collections.Generic;
using UnityEngine;
using Constant;
using Extensions;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    
    public PlayerInput playerInput { get; private set; }
    
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
