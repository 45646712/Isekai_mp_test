using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Extensions;
using Constant;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

using Access = Constant.PlayerDataConstants.DataAccessibility;
using PublicData = Constant.PlayerDataConstants.PublicDataType;

public class SessionListUI : MonoBehaviour, IRefreshable
{
    [SerializeField] private Transform listAnchor;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button changeNicknameButton;//experimental, will move
    [SerializeField] private Button directJoinButton;
    [SerializeField] private Button closeButton;
    
    [SerializeField] private TMP_InputField directJoinInputField;
    [SerializeField] private TMP_InputField changeNicknameInputField;//experimental, will move
    
    public List<SessionDetail> AllRoomsAvailable { get; } = new();
    public List<SessionConstants.SessionPrivacy> filterQuery { get; set; } = new();
    
    private void Awake()
    {
        tutorialButton.onClick.AddListener(() => { });
        refreshButton.onClick.AddListener(() =>
        {
            Refresh().Forget();
        });
        directJoinButton.onClick.AddListener(() =>
        {
            DirectJoin(Int64.Parse(directJoinInputField.text)).Forget();
        });
        closeButton.onClick.AddListener(() =>
        {
            Destroy(UIManager.Instance.AllActiveStepUIs.Pop());
        });
        
        changeNicknameButton.onClick.AddListener(() =>
        {
            PlayerDataManager.Instance.UpdateAndSaveData(Access.Public, PublicData.Name, changeNicknameInputField.text).Forget();
            SessionManager.Instance.UpdateSessionHostInfo();
        });
    }

    private async UniTaskVoid Start()
    {
        RegisterUI();
        
        await Refresh();
        await RefreshUI(cancellationToken:this.GetCancellationTokenOnDestroy(),Time.unscaledTime + SessionConstants.SessionRefreshTimer);
    }

    private async UniTask DirectJoin(Int64 userID)
    {
        Query query = new Query(new List<FieldFilter>
        {
            new(PlayerDataConstants.PublicDataType.UserID.ToString(), userID, FieldFilter.OpOptions.EQ, false)
        }, null, 0, 1);

        List<EntityData> result = await CloudSaveService.Instance.Data.Player.QueryAsync(query, new QueryOptions());
        ISessionInfo targetSession = SessionManager.Instance.AllActiveSessions.Find(x => x.HostId == result[0].Id);
        
        if (result.Count == 0 || targetSession == null)
        {
            LogManager.instance.LogErrorAndShowUI("Player Not Found or Not in Room !");
            return;
        }

        SessionConstants.SessionPrivacy privacy = targetSession.GetPrivacyState();

        switch (privacy)
        {
            case SessionConstants.SessionPrivacy.Public:
            case SessionConstants.SessionPrivacy.Public_friend:
                await SessionManager.Instance.StartClient(targetSession.Id);
                break;
            case SessionConstants.SessionPrivacy.AskToJoin:
            case SessionConstants.SessionPrivacy.AskToJoin_friend:
                CommunicationManager.Instance.isJoinAccessRestricted = false;

                CommunicationManager.Instance.SendMsgToPlayer(AuthenticationService.Instance.PlayerId, CommunicationConstants.MessageType.JoinRequest, result[0].Id).Forget();
                break;
            default:
                LogManager.instance.LogErrorAndShowUI("Room is not available !");
                break;
        }
    }
    
    private async UniTask RefreshUI(CancellationToken cancellationToken , float nextRefreshTime)
    {
        while (Time.unscaledTime < nextRefreshTime)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            await UniTask.NextFrame();
        }
        
        nextRefreshTime = Time.unscaledTime + SessionConstants.SessionRefreshTimer;
        await Refresh();
    }
    
    public async UniTask Refresh()
    {
        await SessionManager.Instance.UpdateSessions();

        //clean up old records
        foreach (SessionDetail element in AllRoomsAvailable)
        {
            PoolManager.Instance.Release(ObjectPoolType.SessionDetail, element.gameObject);
        }
        
        AllRoomsAvailable.Clear();
        
        //sort result by privacy setting (enum)
        List<ISessionInfo> sortedList = new();

        foreach (ISessionInfo element in SessionManager.Instance.AllActiveSessions)
        {
            SessionConstants.SessionPrivacy privacy = element.GetPrivacyState();

            if (filterQuery.Count > 0 && !filterQuery.Contains(privacy))
            {
                continue;
            }
            
            sortedList.Add(element);
        }

        sortedList = sortedList.OrderBy(x=>x.GetPrivacyState()).ToList();
        
        //populate UI using result
        foreach (ISessionInfo element in sortedList)
        {
            if (AllRoomsAvailable.Count < SessionManager.Instance.MaxSessionQuery) //population stops when max query reached
            {
                SessionDetail detail = PoolManager.Instance.Get(ObjectPoolType.SessionDetail, listAnchor).GetComponent<SessionDetail>();
                detail.Init(element);
                AllRoomsAvailable.Add(detail);
            }
        }
    }
    
    public IEnumerator ButtonAccessControl(int seconds)
    {
        refreshButton.interactable = false;
        yield return new WaitForSeconds(seconds);
        refreshButton.interactable = true;
    }
    
    public void RegisterUI()
    {
        UIManager.Instance.AllActiveStepUIs.Push(gameObject);
        UIManager.Instance.AllActiveUIs.Add(UIConstants.NonPooledUITypes.SessionList, gameObject);
    }

    public void UnregisterUI()
    {
        UIManager.Instance.AllActiveUIs.Remove(UIConstants.NonPooledUITypes.SessionList);
    }

    private void OnDestroy()
    {
        UnregisterUI();
    }
}
