using System;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private Button googleLogin;
    [SerializeField] private Button facebookLogin;
    [SerializeField] private Button guestLogin;
    [SerializeField] private Button closeButton;
    
    private void Awake()
    {
        googleLogin.onClick.AddListener(LoginManager.Instance.GoogleLogin);
        facebookLogin.onClick.AddListener(LoginManager.Instance.FacebookLogin);
        guestLogin.onClick.AddListener(LoginManager.Instance.GuestLogin);
        closeButton.onClick.AddListener(RefreshUI);
    }

    private void RefreshUI()
    {
        LoginManager.Instance.RefreshLoginUIState(LoginUIState.PreLogin);
        Destroy(gameObject);
    }
}
