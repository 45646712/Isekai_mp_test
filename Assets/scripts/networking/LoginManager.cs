using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using Unity.Services.Authentication;
using GooglePlayGames;
using Unity.Services.Relay.Models;
using UnityEngine.SceneManagement;

public enum LoginUIState
{
    PreLogin,
    Login
}

public class LoginManager : NetworkBehaviour
{
    public static LoginManager Instance;

    [SerializeField] private GameObject[] preLoginButtons;
    
    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        PlayGamesPlatform.Activate();
        
        InitLoginEvents();
    }
    
    private void InitLoginEvents()
    {
        AuthenticationService.Instance.SignedIn += async() =>
        {
            await SessionManager.Instance.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("v1test", LoadSceneMode.Single);
        };

        AuthenticationService.Instance.SignInFailed += Debug.LogError;
        AuthenticationService.Instance.SignedOut += () => { Debug.Log("Signed out."); };
        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };
    }
    
    public async void GoogleLogin()
    {
        Debug.Log("Google");
    }

    public async void FacebookLogin()
    {
        Debug.Log("Facebook");
    }
    
    public async void GuestLogin() => await AuthenticationService.Instance.SignInAnonymouslyAsync();
    
    public void RefreshLoginUIState(LoginUIState state)
    {
        foreach (GameObject element in preLoginButtons)
        {
            element.SetActive(state == LoginUIState.PreLogin);
        }
    }
}
