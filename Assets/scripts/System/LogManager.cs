using System;
using Constant;
using UnityEngine;

//for behaviours that require monobehavior runtime and debug
public class LogManager : MonoBehaviour
{
    public static LogManager instance;
    
    [SerializeField] private GameObject ErrorPopup;

    private void Awake()
    {
        instance = this;
    }

    public void LogErrorAndShowUI(object message)
    { 
        Debug.LogError(message);
        //Instantiate(ErrorPopup);
        //string a = ErrorKeyConstants.KeyToResponse[]
    }
}