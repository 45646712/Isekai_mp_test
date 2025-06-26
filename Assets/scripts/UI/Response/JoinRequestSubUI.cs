using System;
using System.Collections;
using System.Threading.Tasks;
using Communications;
using Constant;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudCode.Subscriptions;
using UnityEngine;
using UnityEngine.UI;

public class JoinRequestSubUI : MonoBehaviour, IResponse
{
    [SerializeField] private GameObject JoinRequestUI;

    [SerializeField] private Button UIButton;
    [SerializeField] private TMP_Text message;
    [SerializeField] private Button accept;
    [SerializeField] private Button deny;
    [SerializeField] private RectTransform timeGauge;

    private string sendTarget;
    
    private float remainDuration = CommunicationConstants.JoinRequestDuration;
    private float width;
    private float counter;
    
    public void Init(IMessageReceivedEvent evt)
    {
        RegisterUI();
        
        sendTarget = evt.Message;
        width = UIButton.GetComponent<RectTransform>().sizeDelta.x;

        message.text = $"Join Request Received: {remainDuration} sec";

        UIButton.onClick.AddListener(() =>
        {
            //Instantiate(JoinRequestUI);
        });

        accept.onClick.AddListener(async () =>
        {
            CommunicationManager.Instance.SendMsgToPlayer(SessionManager.Instance.CurrentSession.Id, CommunicationConstants.MessageType.JoinAcknowledged, sendTarget);
            Destroy();
        });

        deny.onClick.AddListener(async () =>
        {
            CommunicationManager.Instance.SendMsgToPlayer(SessionManager.Instance.CurrentSession.Id, CommunicationConstants.MessageType.JoinDenied, sendTarget);
            Destroy();
        });
    }

    private async void Update()
    {
        if (remainDuration > 0)
        {
            remainDuration -= Time.deltaTime;
            message.text = $"Join Request Received: {Mathf.CeilToInt(remainDuration)} sec";
        }
        else
        {
            CommunicationManager.Instance.SendMsgToPlayer(SessionManager.Instance.CurrentSession.Id, CommunicationConstants.MessageType.JoinDenied, sendTarget);
            Destroy();
        }

        counter += 0.1f * Time.deltaTime;
        timeGauge.sizeDelta = new Vector2(Mathf.Lerp(0, -width, counter), timeGauge.sizeDelta.y);
    }

    public void RegisterUI() => UIManager.Instance.AllActiveUIs.Add(UIConstant.AllTypes.JoinRequestSubMenu, gameObject);
    public void UnregisterUI() => UIManager.Instance.AllActiveUIs.Remove(UIConstant.AllTypes.JoinRequestSubMenu);
    
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