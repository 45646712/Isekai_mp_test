using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Constant;
using Extensions;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using UnityEngine;

public class SessionManager : NetworkBehaviour
{
    public static SessionManager Instance;
    
    public List<ISessionInfo> AllActiveSessions { get; private set; } = new();
    
    public ISession CurrentSession { get; private set; }
    private ISession previousSession;
    
    private SessionOptions hostOption;
    private QuerySessionsOptions SessionFilterOption;
    
    private void Awake()
    {
        Instance = this;

        hostOption = new SessionOptions
        {
            MaxPlayers = 4,
            IsLocked = false,
            IsPrivate = false,
            Password = "9999" + AccountConstant.BypassSessionPwRestriction,
            SessionProperties =
            {
                { SessionConstants.PropertyKeys.IsAskToJoin.ToString(), new SessionProperty("false") },
                { SessionConstants.PropertyKeys.IsFriendOnly.ToString(), new SessionProperty("false") }
            }
        }.WithRelayNetwork();

        SessionFilterOption = new QuerySessionsOptions()
        {
            Count = 50
        };
    }

    public async Task StartHost()
    {
        previousSession = CurrentSession;
        
        try
        {
            CurrentSession = await MultiplayerService.Instance.CreateSessionAsync(hostOption);
        }
        catch (Exception e)
        {
            if (e.ToString().Contains(SystemConstants.ErrorKey_MultipleOwnership)) // not yet an error
            {
                await LeaveSession(previousSession);
                await StartHost();
                return;
            }
            
            Debug.LogError(e);
            return;
        }
        
        UIManager.Instance.CloseAllUI();
        
        SessionFilterOption.FilterOptions = new()
        {
            new FilterOption(FilterField.Name, CurrentSession.Name, FilterOperation.NotEqual)
        };

        CommunicationManager.Instance.isJoinRequestRestricted = false;
    }

    public async Task StartClient(string sessionID, JoinSessionOptions joinPassword = null) 
    {
        previousSession = CurrentSession;
        
        try
        {
            if (joinPassword != null)
            {
                CurrentSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionID, joinPassword);
            }
            else
            {
                CurrentSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionID);
            }
        }
        catch (Exception e)
        {
            if (e.ToString().Contains(SystemConstants.ErrorKey_MultipleOwnership)) // not yet an error
            {
                await LeaveSession(previousSession);
                await StartClient(sessionID, joinPassword);
                return;
            }
            
            Debug.LogError(e);
            return;
        }
        
        UIManager.Instance.CloseAllUI();
        
        SessionFilterOption.FilterOptions = new()
        {
            new FilterOption(FilterField.Name, CurrentSession.Name, FilterOperation.NotEqual)
        };
    }
    
    public async Task UpdateSessions()
    {
        QuerySessionsResults result = await MultiplayerService.Instance.QuerySessionsAsync(SessionFilterOption);
        AllActiveSessions.Clear();
        AllActiveSessions = new(result.Sessions);
    }
    
    private async Task LeaveSession(ISession previousSession)
    {
        if (previousSession == null)
        {
            return;
        }

        CommunicationManager.Instance.isJoinRequestRestricted = true;
        
        if (IsHost)
        {
            await previousSession.AsHost().DeleteAsync();
        }
        else
        {
            await previousSession.LeaveAsync();
        }
        
        if (NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        previousSession = null;
    }
}