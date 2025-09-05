using System;
using System.Collections.Generic;
using UnityEngine;
using Constant;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [SerializeField] private GameObject controlUI;
    [field: SerializeField] public GameObject IngameCamera { get; private set; }

    public PlayerInput playerInput { get; private set; }
    public CinemachineCamera SpawnedCamera { get; set; }

    private void Awake()
    {
        Instance = this;
        
        playerInput = new PlayerInput();
        playerInput.Enable();
    }
    
    public void EnableControl()
    {
        GameObject obj = Instantiate(controlUI, gameObject.transform);
        UIManager.Instance.AllActiveUIs.Add(UIConstants.AllTypes.ControlOverlay, obj);
    }

    public void DisableControl()
    {
        Destroy(UIManager.Instance.AllActiveUIs[UIConstants.AllTypes.ControlOverlay]);
        UIManager.Instance.AllActiveUIs.Remove(UIConstants.AllTypes.ControlOverlay);
    }
    
    private void OnDestroy()
    {
        playerInput.Disable();
        playerInput.Dispose();
    }
}
