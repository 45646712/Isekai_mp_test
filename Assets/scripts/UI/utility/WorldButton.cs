using System;
using UnityEngine;
using UnityEngine.UI;

public class WorldButton : MonoBehaviour
{
    [SerializeField] private Sprite[] Icons;
    [SerializeField] private Image source;
    [SerializeField] private Button button;
    
    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        button.onClick.AddListener(OnClick);
    }

    private void LateUpdate()
    {
        if (!source)
        {
            return;
        }

        transform.forward = Camera.main.transform.forward; //billboarding
    }

    public void Init(int index, Action buttonEvt) //get enum -> int
    {
        button.onClick.RemoveAllListeners();

        if (index < 0)
        {
            return;
        }

        button.onClick.AddListener(() => buttonEvt());
        button.onClick.AddListener(OnClick);
        source.sprite = Icons[index];
    }

    public void UpdateStatus(bool isActive)
    {
        if (!source.sprite)
        {
            source.enabled = false;
            button.interactable = false;
            return;
        }
        
        source.enabled = isActive;
        button.interactable = isActive;
    }
    
    public void OnClick()
    {
        button.onClick.RemoveAllListeners();
        source.sprite = null;
        source.enabled = false;
        button.interactable = false;
    }
}
