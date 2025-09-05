using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constant;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    public Stack<GameObject> AllActiveStepUIs { get; set; } = new();
    public Dictionary<UIConstants.AllTypes, GameObject> AllActiveUIs { get; set; } = new();
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void CloseAllUI()
    {
        while (AllActiveStepUIs.TryPop(out GameObject element))
        {
            Destroy(element);
        }
        
        foreach (var(key,value) in AllActiveUIs)
        {
            Destroy(value);
        }
        
        AllActiveUIs.Clear();
        AllActiveStepUIs.Clear();
    }
}
