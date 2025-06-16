using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class SessionManager : NetworkBehaviour
{
    public static SessionManager Instance;
    
    public List<ISessionInfo> allActiveSessions = new();

    private SessionOptions hostOption;
    private QuerySessionsOptions SessionFilterOption;
    
    private void Awake()
    {
        Instance = this;
        
        hostOption = new SessionOptions
        {
            MaxPlayers = 4
            //name
            //islocked
            //isprivate
            //password
        }.WithRelayNetwork();
        
        SessionFilterOption = new QuerySessionsOptions()
        {
            Count = 5
            //default amount = 100
            //skip
            //filter options
            //sort options
        };
    }

    public async Task StartHost() => await MultiplayerService.Instance.CreateSessionAsync(hostOption);
    public async Task StartClient(string sessionID) => await MultiplayerService.Instance.JoinSessionByIdAsync(sessionID);

    public async Task GetSessions()
    {
        QuerySessionsResults result = await MultiplayerService.Instance.QuerySessionsAsync(SessionFilterOption);
        allActiveSessions = new(result.Sessions);
    }
}
