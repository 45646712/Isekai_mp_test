using System;
using System.Collections.Generic;
using Constant;
using Cysharp.Threading.Tasks;
using Extensions;
using Unity.Mathematics;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.CloudSave;
//using GooglePlayGames;
using UnityEngine.SceneManagement;

public class LoginManager : NetworkBehaviour
{
    public static LoginManager Instance;
    
    private void Awake()
    {
        Instance = this;
    }

    private async UniTaskVoid Start()
    {
        await UnityServices.InitializeAsync();
        //PlayGamesPlatform.Activate();

        InitLoginEvents();
    }
    

    private void InitLoginEvents()
    {
        AuthenticationService.Instance.SignedIn += UniTask.Action(async () =>
        {
            CloudCodeManager.Instance.Init(); //init modules
            
            await CloudCodeManager.Instance.ValidateAccountData();
            await SessionManager.Instance.UpdateSessionHostInfo();
            await GameDataManager.instance.testcall();
            return;
            await CommunicationManager.Instance.Init();
            await SessionManager.Instance.StartHost();
            
            NetworkManager.Singleton.SceneManager.LoadScene("v1test", LoadSceneMode.Single);
        });

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

    public void GuestLogin() => AuthenticationService.Instance.SignInAnonymouslyAsync().AsUniTask().Forget();
}
