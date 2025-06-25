using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Constant;
using Unity.Netcode;
using Unity.Services.Multiplayer;

public class SessionManager : NetworkBehaviour
{
    public static SessionManager Instance;
    
    public List<ISessionInfo> AllActiveSessions { get; private set; } = new();
    public ISession CurrentSession { get; private set; }

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
            SessionProperties =
            {
                { SessionConstants.PropertyKeys.IsAskToJoin.ToString(), new SessionProperty("true") },
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
        UIManager.Instance.CloseAllUI();
        
        await LeaveSession();
        
        IHostSession result = await MultiplayerService.Instance.CreateSessionAsync(hostOption);
        CurrentSession = result;
        SessionFilterOption.FilterOptions = new()
        {
            new FilterOption(FilterField.Name, CurrentSession.Name, FilterOperation.NotEqual)
        };
        
        CommunicationManager.Instance.isJoinRequestRestricted = false;
    }

    public async Task StartClient(string sessionID)
    {
        UIManager.Instance.CloseAllUI();
        
        await LeaveSession();
        
        ISession result = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionID);
        CurrentSession = result;
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
    
    private async Task LeaveSession()
    {
        if (CurrentSession == null)
        {
            return;
        }
        
        CommunicationManager.Instance.isJoinRequestRestricted = true;
        
        if (IsHost)
        {
            await CurrentSession.AsHost().DeleteAsync();
        }
        else
        {
            await CurrentSession.LeaveAsync();
        }
        
        if (NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}