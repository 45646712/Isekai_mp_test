using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using Constant;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Services.CloudCode.Subscriptions;
using UnityEngine;
using UnityEngine.UI;

public class JoinRequestSubUI : MonoBehaviour , IResponse
{
    [SerializeField] private Button UIButton;
    [SerializeField] private TMP_Text message;
    [SerializeField] private Button accept;
    [SerializeField] private Button deny;
    [SerializeField] private RectTransform timeGauge;

    public Queue<IMessageReceivedEvent> StoredRequests { get; set; } = new();

    private string sendTarget;
    private float width;
    
    private void Start()
    {
        width = timeGauge.GetComponent<RectTransform>().rect.width;
    }

    public void Init(IMessageReceivedEvent evt)
    {
        RegisterUI();
        
        sendTarget = evt.Message;
        
        UIButton.onClick.AddListener(() =>
        {
            JoinRequestUI ui = UIManager.Instance.SpawnUI(UIConstants.NonPooledUITypes.JoinRequestMenu).GetComponent<JoinRequestUI>();
            ui.Init(evt);
            ui.Init(StoredRequests);

            Destroy();
        });

        accept.onClick.AddListener(() =>
        {
            CommunicationManager.Instance.SendMsgToPlayer(SessionManager.Instance.CurrentSession.Id, CommunicationConstants.MessageType.JoinAcknowledged, sendTarget).Forget();
            Destroy();
        });

        deny.onClick.AddListener(() =>
        {
            CommunicationManager.Instance.SendMsgToPlayer(SessionManager.Instance.CurrentSession.Id, CommunicationConstants.MessageType.JoinDenied, sendTarget).Forget();
            Destroy();
        });
        
        StartCoroutine(UpdateUI(evt.Time.AddSeconds(CommunicationConstants.JoinRequestDuration)));
    }

    private IEnumerator UpdateUI(DateTimeOffset endTime)
    {
        yield return new WaitWhile(() =>
        {
            double lerpIndex = (endTime - DateTimeOffset.Now).TotalMilliseconds / 1000 / CommunicationConstants.JoinRequestDuration;

            message.text = $"Join Request Received: {Mathf.CeilToInt(CommunicationConstants.JoinRequestDuration)} sec";
            timeGauge.sizeDelta = new Vector2(Mathf.Lerp(-width, 0, (float)lerpIndex), timeGauge.sizeDelta.y);

            return DateTimeOffset.Now < endTime;
        });
        
        CommunicationManager.Instance.SendMsgToPlayer(SessionManager.Instance.CurrentSession.Id, CommunicationConstants.MessageType.JoinDenied, sendTarget).Forget();
        Destroy();
    }

    public void RegisterUI() => UIManager.Instance.AllActiveUIs.Add(UIConstants.NonPooledUITypes.JoinRequestSubMenu, gameObject);
    public void UnregisterUI() => UIManager.Instance.AllActiveUIs.Remove(UIConstants.NonPooledUITypes.JoinRequestSubMenu);
    
    private void Destroy()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        UnregisterUI();
    }
}