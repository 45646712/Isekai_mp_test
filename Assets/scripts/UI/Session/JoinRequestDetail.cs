using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Communications;
using Constant;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.Subscriptions;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
using UnityEngine;
using UnityEngine.UI;

public class JoinRequestDetail : MonoBehaviour
{
    [SerializeField] private TMP_Text nickname;
    [SerializeField] private TMP_Text userID;
    [SerializeField] private Button accept;
    [SerializeField] private Button deny;
    [SerializeField] private RectTransform timeGauge;
    
    private string sendTarget;
    private float width;
    
    private void Start()
    {
        width = timeGauge.GetComponent<RectTransform>().rect.width;
    }

    public async UniTask Init(IMessageReceivedEvent evt)
    {
        sendTarget = evt.Message;

        Dictionary<string, Item> targetInfo = await CloudSaveService.Instance.Data.Player.LoadAsync(PlayerDataConstant.PublicKeys, new LoadOptions(new PublicReadAccessClassOptions(evt.Message)));

        Int64 UID = targetInfo[PlayerDataConstant.PublicDataType.UserID.ToString()].Value.GetAs<Int64>();
        byte playerLevel = targetInfo[PlayerDataConstant.PublicDataType.Lv.ToString()].Value.GetAs<byte>();
        string playerName = targetInfo[PlayerDataConstant.PublicDataType.Name.ToString()].Value.GetAs<string>();
        
        nickname.text = $"Lv.{playerLevel} {playerName}";
        userID.text = $"UserID : {UID}";
        
        accept.onClick.RemoveAllListeners();
        accept.onClick.AddListener(() =>
        {
            CommunicationManager.Instance.SendMsgToPlayer(SessionManager.Instance.CurrentSession.Id, CommunicationConstants.MessageType.JoinAcknowledged, sendTarget).Forget();
            Destroy();
        });

        deny.onClick.RemoveAllListeners();
        deny.onClick.AddListener(() =>
        {
            CommunicationManager.Instance.SendMsgToPlayer(SessionManager.Instance.CurrentSession.Id, CommunicationConstants.MessageType.JoinDenied, sendTarget).Forget();
            Destroy();
        });

        StartCoroutine(UpdateUI(evt.Time.AddSeconds(CommunicationConstants.JoinRequestDuration)));
    }

    private IEnumerator UpdateUI(DateTime endTime)
    {
        yield return new WaitWhile(() =>
        {
            double lerpIndex = (endTime - DateTime.Now).TotalMilliseconds / 1000 / CommunicationConstants.JoinRequestDuration;
            timeGauge.sizeDelta = new Vector2(Mathf.Lerp(-width, 0, (float)lerpIndex), timeGauge.sizeDelta.y);
            
            return DateTime.Now < endTime;
        });
        
        CommunicationManager.Instance.SendMsgToPlayer(SessionManager.Instance.CurrentSession.Id, CommunicationConstants.MessageType.JoinDenied, sendTarget).Forget();
        Destroy();
    }

    private void Destroy()
    {
        StopAllCoroutines();
        PoolManager.Instance.Release(ObjectPoolType.RequestDetail, gameObject); //disabled (pooling)
    }
}