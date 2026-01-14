using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Models;
using NUnit.Framework;
using Unity.Cinemachine;
using UnityEngine;

public class Field : MonoBehaviour
{
    [field: SerializeField] public CropSlot[] Crops { get; private set; } //expose crop slots
    
    [SerializeField] private BatchPlantTerminal terminal;

    private void Awake()
    {
        terminal.gameObject.SetActive(false);
    }

    public void Init()
    {
        terminal.gameObject.SetActive(true);

        if (SessionManager.Instance.CurrentSession.IsHost)
        {
            terminal.Init(Crops);
        }
    }
}