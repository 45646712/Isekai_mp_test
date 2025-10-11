using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Constant;
using Extensions;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.CloudCode.Subscriptions;

public class CommunicationManager : NetworkBehaviour
{
    public static CommunicationManager Instance;
    
    //access control flags
    public bool isJoinRequestRestricted { get; set; } // block invalid join request after host left
    public bool isJoinAccessRestricted { get; set; } // block possible incoming join information after joined a room
    
    private void Awake()
    {
        Instance = this;
    }

    public async UniTask Init()
    {
        await this.SubscribeToMessage();
        await this.SubscribeToBroadCast();
    }

    public async UniTask OnMessageReceived(CommunicationConstants.MessageType type, IMessageReceivedEvent evt)
    {
        switch (type)
        {
            case CommunicationConstants.MessageType.JoinRequest:
                if (isJoinRequestRestricted)
                {
                    await this.SendMsgToPlayer(AuthenticationService.Instance.PlayerId, CommunicationConstants.MessageType.JoinTimeout, evt.Message);
                    return;
                }
                
                bool isSubUIExist = UIManager.Instance.AllActiveUIs.TryGetValue(UIConstants.NonPooledUITypes.JoinRequestSubMenu, out GameObject joinRequestSubUI);
                bool isMainUIExist = UIManager.Instance.AllActiveUIs.TryGetValue(UIConstants.NonPooledUITypes.JoinRequestMenu, out GameObject joinRequestUI);
                
                if (isSubUIExist)
                {
                    joinRequestSubUI.GetComponent<JoinRequestSubUI>().StoredRequests.Enqueue(evt);
                    break;
                }
                
                if (isMainUIExist)
                {
                    joinRequestUI.GetComponent<JoinRequestUI>().Init(evt);
                    break;
                }
                
                Instantiate(UIManager.Instance.UIPrefabs[UIConstants.NonPooledUITypes.JoinRequestSubMenu]).GetComponent<JoinRequestSubUI>().Init(evt);
                break;
            case CommunicationConstants.MessageType.JoinDenied:
                if (isJoinAccessRestricted)
                {
                    break;
                }
                
                Debug.LogError("Join Request Denied!");
                
                UIManager.Instance.AllActiveUIs[UIConstants.NonPooledUITypes.SessionList].GetComponent<SessionListUI>().AllRoomsAvailable.Find(x => x.Info.Id == evt.Message).RefreshButton();
                break;
            case CommunicationConstants.MessageType.JoinTimeout:
                SessionDetail roomUI = UIManager.Instance.AllActiveUIs[UIConstants.NonPooledUITypes.SessionList].GetComponent<SessionListUI>().AllRoomsAvailable.Find(x => x.Info.HostId == evt.Message);
                
                Debug.LogError("Session Timeout!");
                
                if (roomUI != null)
                {
                    roomUI.gameObject.SetActive(false);
                }
                break;
            case CommunicationConstants.MessageType.JoinAcknowledged:
                if (isJoinAccessRestricted)
                {
                    break;
                }
                isJoinAccessRestricted = true;
                
                await SessionManager.Instance.StartClient(evt.Message);
                break;
            case CommunicationConstants.MessageType.SessionTerminated:
                Debug.LogError("host left, session terminated\n returning to own session.");
                await SessionManager.Instance.StartHost();
                break;
            case CommunicationConstants.MessageType.Kicked:
                Debug.LogError("You have been kicked by the host!\n returning to own session");
                await SessionManager.Instance.StartHost();
                break;
        }
    }

    public async UniTask OnBroadCastReceived(CommunicationConstants.BroadcastType type, IMessageReceivedEvent evt){}
}