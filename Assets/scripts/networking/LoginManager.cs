using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using Unity.Services.Authentication;
using GooglePlayGames;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private GameObject LoginUI;
    
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        PlayGamesPlatform.Activate();
        
        InitLoginEvents();
        Instantiate(LoginUI);
    }

    private void InitLoginEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            NetworkManager.Singleton.SceneManager.LoadScene("v1test",LoadSceneMode.Single);
        };

        AuthenticationService.Instance.SignInFailed += Debug.LogError;
        AuthenticationService.Instance.SignedOut += () => { Debug.Log("Signed out."); };
        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };
    }

    public async Task GoogleLogin()
    {
        
    }

    public async Task FacebookLogin()
    {
        
    }
    
    public async Task GuestLogin()
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
