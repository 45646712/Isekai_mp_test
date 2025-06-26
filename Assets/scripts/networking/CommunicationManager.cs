using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Constant;
using Communications;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.CloudCode.Subscriptions;

public class CommunicationManager : NetworkBehaviour
{
    public static CommunicationManager Instance;

    [SerializeField,SerializedDictionary("Message Type", "Prefab")]
    private SerializedDictionary<CommunicationConstants.MessageType, GameObject> allMessageUI = new();

    //access control flags
    public bool isJoinRequestRestricted { get; set; } // block invalid join request after host left
    public bool isJoinAccessRestricted { get; set; } // block possible incoming join information after joined a room
    
    private void Awake()
    {
        Instance = this;
    }

    public async Task Init()
    {
        await this.SubscribeToMessage();
        await this.SubscribeToBroadCast();
    }

    public async Task OnMessageReceived(CommunicationConstants.MessageType type, IMessageReceivedEvent evt)
    {
        switch (type)
        {
            case CommunicationConstants.MessageType.JoinRequest:
                if (isJoinRequestRestricted)
                {
                    await this.SendMsgToPlayer(AuthenticationService.Instance.PlayerId, CommunicationConstants.MessageType.JoinTimeout, evt.Message);
                    return;
                }

                allMessageUI.TryGetValue(type, out GameObject messageUI);
                Instantiate(messageUI).GetComponent<IResponse>().Init(evt);
                break;
            case CommunicationConstants.MessageType.JoinDenied:
                if (isJoinAccessRestricted)
                {
                    break;
                }
                
                Debug.LogError("Join Request Denied!");
                
                UIManager.Instance.AllActiveUIs[UIConstant.AllTypes.SessionList].GetComponent<SessionListUI>().AllRoomsAvailable.Find(x => x.Info.Id == evt.Message).RefreshButton();;
                break;
            case CommunicationConstants.MessageType.JoinTimeout:
                SessionDetail roomUI = UIManager.Instance.AllActiveUIs[UIConstant.AllTypes.SessionList].GetComponent<SessionListUI>().AllRoomsAvailable.Find(x => x.Info.HostId == evt.Message);
                
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
        }
    }
    
    public async Task OnBroadCastReceived(CommunicationConstants.BroadcastType type, IMessageReceivedEvent evt){}
}