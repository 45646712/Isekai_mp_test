using System;
using Constant;
using UnityEngine;
using UnityEngine.UI;

public class SessionButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { UIManager.Instance.SpawnUI(UIConstants.NonPooledUITypes.SessionList); }); 
    }
}
