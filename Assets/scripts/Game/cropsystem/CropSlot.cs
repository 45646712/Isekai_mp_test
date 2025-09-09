using System;
using System.Collections.Generic;
using System.IO;
using Constant;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Models;
using UnityEngine.UI;

public class CropSlot : MonoBehaviour
{
    [SerializeField] private WorldButton InteractButton;
    
    public CropModel.CropData data { get; private set; }

    public bool isOccupied { get; private set; }

    private void Start() => GetComponent<BoxCollider>().enabled = SessionManager.Instance.CurrentSession.IsHost;

    public void Init(CropSO baseData, DateTimeOffset matureTime, CropConstants.CropStatus status = CropConstants.CropStatus.Growing)
    {
        data = new(baseData, matureTime, status);
        
        GetComponent<MeshFilter>().mesh = data.Appearance[status];
        
        if (data.Material.Count != 0)
        {
            GetComponent<MeshRenderer>().materials = data.Material[status];
        }

        switch (data.Status)
        {
            case CropConstants.CropStatus.Null:
                InteractButton.Init((int)status, () => { }); //summon UI
                break;
            case CropConstants.CropStatus.Matured:
                InteractButton.Init((int)status, () => { CropManager.Instance.Harvest(CropManager.Instance.AllCrops.IndexOf(this)).Forget(); });
                break;
        }
        
        isOccupied = true;
    }

    public void Reset()
    {
        data.Reset();

        GetComponent<MeshFilter>().mesh = null;
        GetComponent<MeshRenderer>().materials = Array.Empty<Material>();

        isOccupied = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            InteractButton.UpdateStatus(true);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            InteractButton.UpdateStatus(false);
        }
    }
}