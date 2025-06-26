using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Constant;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [field: SerializeField, SerializedDictionary("Button type" , "Button")]
    private SerializedDictionary<LoginConstants.LoginButton, Button> loginButtons = new();
    
    private Dictionary<LoginConstants.PreLoginButton, Button> preLoginButtons = new();

    public void Init(Dictionary<LoginConstants.PreLoginButton, Button> preLoginButtons)
    {
        this.preLoginButtons = preLoginButtons;

        loginButtons[LoginConstants.LoginButton.Google].onClick.AddListener(LoginManager.Instance.GoogleLogin);
        loginButtons[LoginConstants.LoginButton.Facebook].onClick.AddListener(LoginManager.Instance.FacebookLogin);
        loginButtons[LoginConstants.LoginButton.Guest].onClick.AddListener(LoginManager.Instance.GuestLogin);
        loginButtons[LoginConstants.LoginButton.Close].onClick.AddListener(() =>
        {
            RefreshLoginUIState(true);
            Destroy(gameObject);
        });
    }

    public void RefreshLoginUIState(bool resetToPreLogin)
    {
        foreach (var (key, value) in preLoginButtons)
        {
            value.gameObject.SetActive(resetToPreLogin);
        }
    }
}