using System;
using AYellowpaper.SerializedCollections;
using Constant;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class StartLogin : MonoBehaviour
{
    [SerializeField] private GameObject loginUI;

    [field: SerializeField, SerializedDictionary("Button type" , "Button")]
    private SerializedDictionary<LoginConstants.PreLoginButton, Button> preLoginButtons = new();
    
    public void InitLoginUI() //register on inspector
    {
        LoginUI ui = Instantiate(loginUI).GetComponent<LoginUI>();
        ui.Init(preLoginButtons);
        ui.RefreshLoginUIState(false);
    }
}
