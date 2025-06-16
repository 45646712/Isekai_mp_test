using System;
using UnityEngine;
using UnityEngine.UI;

public class SessionButton : MonoBehaviour
{
    [SerializeField] private GameObject sessionListUI;
    
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { Instantiate(sessionListUI); });
    }
}
