using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Multiplayer;
using UnityEngine.UI;

public class SessionManager : MonoBehaviour
{
    [SerializeField] private Button Host;
    [SerializeField] private Button Client;
    
    async void Start()
    {
        Host.onClick.AddListener(StartHost);
        Client.onClick.AddListener(StartClient);
        
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    private async void StartHost()
    {
        SessionOptions option = new SessionOptions
        {
            MaxPlayers = 4
            //name
            //islocked
            //isprivate
            //password
        }.WithRelayNetwork();
        
        await MultiplayerService.Instance.CreateOrJoinSessionAsync("1234",option);
    }

    private async void StartClient()
    {
        QuerySessionsOptions option = new QuerySessionsOptions()
        {
            Count = 5
            //default amount = 100
            //skip
            //filter options
            //sort options
        };

        QuerySessionsResults result = await MultiplayerService.Instance.QuerySessionsAsync(option);
        List<ISessionInfo> sessions = new(result.Sessions);

        foreach (ISessionInfo element in sessions)
        {
            print($"{element.Name} / {element.Id}");
        }

        await MultiplayerService.Instance.JoinSessionByIdAsync(sessions[0].Id);
    }
}
