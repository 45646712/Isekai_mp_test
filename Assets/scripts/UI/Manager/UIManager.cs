using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using Constant;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [field: SerializeField, SerializedDictionary("UIType", "UI")] private SerializedDictionary<UIConstants.NonPooledUITypes, GameObject> UIPrefabs { get;  set; }

    public Stack<GameObject> AllActiveStepUIs { get; set; } = new();
    public Dictionary<UIConstants.NonPooledUITypes, GameObject> AllActiveUIs { get; set; } = new();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameObject SpawnUI(UIConstants.NonPooledUITypes type) => AllActiveUIs.ContainsKey(type) ? null : Instantiate(UIPrefabs[type]);

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
