using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class SessionManager : NetworkBehaviour
{
    [SerializeField] private Button host;
    [SerializeField] private Button client;
    
    async void Start()
    {
        host.onClick.AddListener(StartHost);
        client.onClick.AddListener(StartClient);
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
        
        await MultiplayerService.Instance.CreateSessionAsync(option);
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
