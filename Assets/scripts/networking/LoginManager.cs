using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constant;
using Cysharp.Threading.Tasks;
using Extensions;
using Newtonsoft.Json;
using Unity.Cinemachine;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
//using GooglePlayGames;
using UnityEngine.SceneManagement;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;

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
            await PlayerDataManager.Instance.LoadAllData();
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
