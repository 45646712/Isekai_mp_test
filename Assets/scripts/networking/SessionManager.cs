using System;
using System.Collections.Generic;
using Extensions;
using Cysharp.Threading.Tasks;
using Constant;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : NetworkBehaviour
{
    public static SessionManager Instance;
    
    public List<ISessionInfo> AllActiveSessions { get; private set; } = new();
    
    public ISession CurrentSession { get; private set; }

    public SessionOptions hostOption { get; set; }
    private QuerySessionsOptions SessionFilterOption;

    public int MaxSessionQuery = 50;

    private void Awake()
    {
        Instance = this;

        hostOption = new SessionOptions
        {
            MaxPlayers = SessionConstants.MaxSessionPlayers,
            SessionProperties =
            {
                { SessionConstants.PropertyKeys.IsAskToJoin.ToString(), new SessionProperty("false") },
                { SessionConstants.PropertyKeys.IsFriendOnly.ToString(), new SessionProperty("false") }
            }
        }.WithRelayNetwork();

        SessionFilterOption = new QuerySessionsOptions()
        {
            Count = MaxSessionQuery
        };
    }
    
    public async UniTask StartHost(SessionConstants.SessionPrivacy privacy = SessionConstants.SessionPrivacy.SinglePlayer, string password = null)
    {
        try
        {   
            NetworkManager.Singleton.Shutdown();
            
            UIManager.Instance.CloseAllUI();
            
            ISession newSession = await MultiplayerService.Instance.CreateSessionAsync(hostOption);
            
            await LeaveSession(SessionConstants.SessionOwnership.Host);
            CurrentSession = newSession;
            CurrentSession.SetPrivacyState(privacy, password);

            NetworkManager.Singleton.SceneManager.LoadScene(SceneConstants.GameScene, LoadSceneMode.Single);
        }
        catch (SessionException e)
        {
            LogManager.instance.LogErrorAndShowUI(e.Error.ToString());
            return;
        }
        
        SessionFilterOption.FilterOptions = new()
        {
            new FilterOption(FilterField.Name, CurrentSession.Name, FilterOperation.NotEqual)
        };

        CommunicationManager.Instance.isJoinRequestRestricted = false;
    }
    
    public async UniTask StartClient(string sessionID, JoinSessionOptions joinPassword = null)
    {
        try
        {
            NetworkManager.Singleton.Shutdown();

            UIManager.Instance.CloseAllUI();

            ISession newSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionID,joinPassword);
            
            await LeaveSession(SessionConstants.SessionOwnership.Client);
            CurrentSession = newSession;
        }
        catch (Exception e)
        {
            List<string> result = await MultiplayerService.Instance.GetJoinedSessionIdsAsync();

            if (result.Count == 0)
            {
                await StartHost();
            }
            else
            {
                ISession previousSession = await MultiplayerService.Instance.ReconnectToSessionAsync(result[0]);
                if (previousSession.Host == AuthenticationService.Instance.PlayerId)
                {
                    NetworkManager.Singleton.StartHost();
                }
                else
                {
                    NetworkManager.Singleton.StartClient();
                }
            }

            LogManager.instance.LogErrorAndShowUI(e);
            return;
        }
        
        SessionFilterOption.FilterOptions = new()
        {
            new FilterOption(FilterField.Name, CurrentSession.Name, FilterOperation.NotEqual)
        };
    }

    public async UniTask UpdateSessions()
    {
        QuerySessionsResults result = await MultiplayerService.Instance.QuerySessionsAsync(SessionFilterOption);
        AllActiveSessions.Clear();
        AllActiveSessions = new(result.Sessions);
    }
    
    private async UniTask LeaveSession(SessionConstants.SessionOwnership ownership)
    {
        CommunicationManager.Instance.isJoinRequestRestricted = true;

        if (CurrentSession == null)
        {
            return;
        }

        if (CurrentSession.Host == AuthenticationService.Instance.PlayerId)
        {
            foreach (IReadOnlyPlayer element in CurrentSession.Players)
            {
                if (element.Id == AuthenticationService.Instance.PlayerId)
                {
                    continue;
                }

                await CommunicationManager.Instance.SendMsgToPlayer(CommunicationConstants.MessageType.SessionTerminated, element.Id);
            }
            
            await CurrentSession.AsHost().LeaveAsync();
        }
        else
        {
            await CurrentSession.LeaveAsync();
        }
    }
}