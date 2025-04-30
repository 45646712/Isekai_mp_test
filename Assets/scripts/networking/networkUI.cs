using System;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

public class networkUI : MonoBehaviour
{
    [SerializeField] private Button Host;
    [SerializeField] private Button Client;
    
    private void Start()
    {
        Host.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        Client.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
