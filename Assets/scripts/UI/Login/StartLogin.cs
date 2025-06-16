using System;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class StartLogin : MonoBehaviour
{
    [SerializeField] private GameObject loginUI;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnstartLogin);
    }

    private void OnstartLogin()
    {
        Instantiate(loginUI);
        
        LoginManager.Instance.RefreshLoginUIState(LoginUIState.Login);
    }
}
