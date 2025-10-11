using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using Constant;
using Cysharp.Threading.Tasks;
using Unity.Services.CloudCode.Subscriptions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class JoinRequestUI : MonoBehaviour , IMultiResponse
{
    [SerializeField] private Transform spawnAnchor;
    [SerializeField] private Button closeButton;
    
    private Queue<IMessageReceivedEvent> storedRequest { get; } = new();

    private void Awake()
    {
        RegisterUI();
        
        closeButton.onClick.AddListener(() =>
        {
            Destroy(UIManager.Instance.AllActiveUIs[UIConstants.NonPooledUITypes.JoinRequestMenu]);
        });
    }

    public void Init(IMessageReceivedEvent evt)
    {
        storedRequest.Enqueue(evt);

        Refresh();
    }

    public void Init(Queue<IMessageReceivedEvent> evt)
    {
        while (evt.TryDequeue(out IMessageReceivedEvent result))
        {
            storedRequest.Enqueue(result);
        }

        Refresh();
    }
    
    private void Refresh()
    {
        while (storedRequest.TryDequeue(out IMessageReceivedEvent result))
        {
            PoolManager.Instance.Get(ObjectPoolType.RequestDetail, spawnAnchor).GetComponent<JoinRequestDetail>().Init(result).Forget();
        }
    }

    public void RegisterUI() => UIManager.Instance.AllActiveUIs.Add(UIConstants.NonPooledUITypes.JoinRequestMenu, gameObject);
    public void UnregisterUI() => UIManager.Instance.AllActiveUIs.Remove(UIConstants.NonPooledUITypes.JoinRequestMenu);

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        UnregisterUI();
    }
}