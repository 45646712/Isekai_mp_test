using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constant;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    //joinrequestsubUI : normal
    //joinrequestUI : normal
    //sessionListUI : step
    public Stack<GameObject> AllActiveStepUIs { get; set; } = new();
    public Dictionary<UIConstant.AllTypes, GameObject> AllActiveUIs { get; set; } = new();
    
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
        
        foreach (KeyValuePair<UIConstant.AllTypes, GameObject> element in AllActiveUIs)
        {
            Destroy(element.Value);
        }
    }
}
