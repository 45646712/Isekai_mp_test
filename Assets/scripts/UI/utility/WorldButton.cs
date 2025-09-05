using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldButton : MonoBehaviour
{
    [SerializeField] private Sprite[] Icons;

    private Sprite source;
    
    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        source = GetComponentInChildren<Image>().sprite;
    }

    public void Init(int index) //get enum -> int
    {
        if (index < 0)
        {
            
        }
        else
        {
            
        }
    }
    
    public void OnClick()
    {
        Debug.Log("passed");
    }
}
