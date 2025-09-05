using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using Constant;
using Extensions;
using Communications;
using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine.UI;
using Unity.Services.Multiplayer;
using Exception = System.Exception;

public class SessionDetail : MonoBehaviour
{
    [field: SerializeField, SerializedDictionary("Privacy State", "icon")]
    private SerializedDictionary<SessionConstants.SessionPrivacy, Sprite> allIcons = new();
    
    [SerializeField] private TMP_Text nickname;
    [SerializeField] private TMP_Text ID;
    [SerializeField] private TMP_Text population;
    [SerializeField] private Image privacyIcon;
    [SerializeField] private TMP_Text privacyStateText;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_Text joinText;

    public ISessionInfo Info { get; private set; }

    public void Init(ISessionInfo info)
    {
        Info = info;
        
        SessionConstants.SessionPrivacy state = info.GetPrivacyState();

        SessionModel.SessionHostInfo hostInfo = JsonConvert.DeserializeObject<SessionModel.SessionHostInfo>(info.GetProperty(SessionConstants.PropertyKeys.SessionHostInfo));
        
        nickname.text = $"Lv.{hostInfo.PlayerLevel} {hostInfo.PlayerName}";
        ID.text = $"UserID : {hostInfo.UserID}";
        population.text = $"{info.MaxPlayers - info.AvailableSlots} / {info.MaxPlayers}";
        privacyIcon.sprite = allIcons[state];
        privacyStateText.text = SessionConstants.PrivacyStateToString[state];

        switch (state)
        {
            case SessionConstants.SessionPrivacy.AskToJoin:
            case SessionConstants.SessionPrivacy.AskToJoin_friend:
                joinText.fontSize = 48;
                joinText.text = "Request Join";
                break;
            case SessionConstants.SessionPrivacy.Private:
                password.gameObject.SetActive(true);
                break;
        }

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() =>
        {
            switch (state)
            {
                case SessionConstants.SessionPrivacy.AskToJoin:
                case SessionConstants.SessionPrivacy.AskToJoin_friend:
                    CommunicationManager.Instance.isJoinAccessRestricted = false;

                    CommunicationManager.Instance.SendMsgToPlayer(AuthenticationService.Instance.PlayerId, CommunicationConstants.MessageType.JoinRequest, info.HostId).Forget();

                    joinText.text = "Waiting Reply...";
                    joinButton.interactable = false;
                    
                    StartCoroutine(localRefresh());//safeguard to prevent no response
                    return;
                case SessionConstants.SessionPrivacy.Private:
                    SessionManager.Instance.StartClient(info.Id, new JoinSessionOptions { Password = password.text + AccountConstant.BypassSessionPwRestriction }).Forget();
                    break;
                default:
                    SessionManager.Instance.StartClient(info.Id).Forget();
                    break;
            }
        });
    }
    
    private IEnumerator localRefresh()
    {
        yield return new WaitForSeconds(15);
        RefreshButton();
    }
    
    public void RefreshButton()
    {
        joinText.text = "Request Join";
        joinButton.interactable = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}