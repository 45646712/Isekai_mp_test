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
    public bool isJoinAccessRestricted { get; set; } // block multiple possible incoming joinAck
    
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
                    await this.SendMsgToPlayer(AuthenticationService.Instance.PlayerId, CommunicationConstants.MessageType.JoinDenied, evt.Message);
                    return;
                }

                allMessageUI.TryGetValue(type, out GameObject messageUI);
                Instantiate(messageUI).GetComponent<IResponse>().Init(evt);
                break;
            case CommunicationConstants.MessageType.JoinDenied:
                Debug.LogError("Join Request Denied or Timeout!");
                UIManager.Instance.AllActiveUIs[UIConstant.AllTypes.SessionList].GetComponent<SessionListUI>().RefreshActivatedButton(evt.Message);
                break;
            case CommunicationConstants.MessageType.JoinAcknowledged:
                if (isJoinAccessRestricted)
                {
                    return;
                }
                isJoinAccessRestricted = true;
                
                await SessionManager.Instance.StartClient(evt.Message);
                break;
        }
    }
    
    public async Task OnBroadCastReceived(CommunicationConstants.BroadcastType type, IMessageReceivedEvent evt){}
}