using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Constant;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [SerializeField] private GameObject joystick;
    
    public GameObject IngameCamera;
    public PlayerInput playerInput { get; private set; }
    public VariableJoystick joystickCore { get; private set; }

    public CinemachineCamera SpawnedCamera { get; set; }

    private void Awake()
    {
        Instance = this;
        
        playerInput = new PlayerInput();
        playerInput.Enable();
    }
    
    public void EnableControl()
    {
        GameObject obj = Instantiate(joystick, gameObject.transform);
        UIManager.Instance.AllActiveUIs.Add(UIConstant.AllTypes.ControlOverlay, obj);
        joystickCore = obj.GetComponentInChildren<VariableJoystick>();
    }

    public void DisableControl()
    {
        Destroy(UIManager.Instance.AllActiveUIs[UIConstant.AllTypes.ControlOverlay]);
        UIManager.Instance.AllActiveUIs.Remove(UIConstant.AllTypes.ControlOverlay);
    }
    
    private void OnDestroy()
    {
        playerInput.Disable();
        playerInput.Dispose();
    }
}
