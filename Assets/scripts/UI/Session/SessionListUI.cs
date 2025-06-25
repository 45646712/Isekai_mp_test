using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constant;
using Unity.Netcode;
using Unity.Services.CloudCode.Subscriptions;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class SessionListUI : MonoBehaviour, IRefreshable
{
    [SerializeField] private GameObject sessionDetailUI;
    [SerializeField] private GameObject listAnchor;
    [SerializeField] private Button closeButton;
    
    private List<SessionDetail> AllRoomsAvailable { get; set; } = new();

    private int SessionRefreshTimer = 30; //await option
    private float counter = 0;
    
    private void Awake()
    {
        closeButton.onClick.AddListener(() => { Destroy(UIManager.Instance.AllActiveStepUIs.Pop()); });
    }

    private async void Start()
    {
        RegisterUI();
        await Refresh();
    }

    private async void Update()
    {
        if (counter <= SessionRefreshTimer)
        {
            counter += Time.deltaTime;
        }
        else
        {
            counter = 0;
            await Refresh();
        }
    }

    public void RefreshActivatedButton(string hostID)
    {
        foreach (SessionDetail element in AllRoomsAvailable)
        {
            if (element.HostID == hostID)
            {
                element.RefreshButton();
            }
        }
    }
    
    public async Task Refresh()
    {
        await SessionManager.Instance.UpdateSessions();
        
        if (AllRoomsAvailable.Count != SessionManager.Instance.AllActiveSessions.Count)
        {
            foreach (SessionDetail element in AllRoomsAvailable)
            {
                Destroy(element.gameObject);
            }
            
            AllRoomsAvailable.Clear();
            
            foreach (ISessionInfo element in SessionManager.Instance.AllActiveSessions)
            {
                SessionDetail detail = Instantiate(sessionDetailUI, listAnchor.transform).GetComponent<SessionDetail>();
                detail.Init(element);
                AllRoomsAvailable.Add(detail);
            }
            
        }
        
        for (int i = 0; i < SessionManager.Instance.AllActiveSessions.Count; i++)
        {
            AllRoomsAvailable[i].Init(SessionManager.Instance.AllActiveSessions[i]); //implicit object pooling
        }
    }

    public void RegisterUI()
    {
        UIManager.Instance.AllActiveStepUIs.Push(gameObject);
        UIManager.Instance.AllActiveUIs.Add(UIConstant.AllTypes.SessionList, gameObject);
    }

    public void UnregisterUI()
    {
        UIManager.Instance.AllActiveUIs.Remove(UIConstant.AllTypes.SessionList);
    }
}
