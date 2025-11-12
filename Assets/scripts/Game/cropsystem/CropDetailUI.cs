using System;
using Constant;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CropDetailUI : MonoBehaviour, IGeneric
{
    [SerializeField] private TMP_Text topic;
    [SerializeField] private TMP_Text content;
    [SerializeField] private Button closeButton;
    
    public void Init(string topic, string content)
    {
        this.topic.text = topic;
        this.content.text = content;
    }
    
    public void RegisterUI() => UIManager.Instance.AllActiveUIs.Add(UIConstants.NonPooledUITypes.CropDetailUI, gameObject);
    public void UnregisterUI() => UIManager.Instance.AllActiveUIs.Remove(UIConstants.NonPooledUITypes.CropDetailUI);

    public void Destroy() => Destroy(gameObject);

    private void OnDestroy()
    {
        UnregisterUI();
    }
}
