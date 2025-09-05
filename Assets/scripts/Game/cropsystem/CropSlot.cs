using System;
using System.Collections.Generic;
using System.IO;
using Constant;
using Newtonsoft.Json;
using UnityEngine;
using Models;

public class CropSlot : MonoBehaviour
{
    public CropModel.CropData data { get; private set; }

    public bool isOccupied { get; private set; }

    public void Init(CropSO baseData, DateTimeOffset matureTime, CropConstants.CropStatus status = CropConstants.CropStatus.Growing)
    {
        data = new(baseData, matureTime, status);
        
        GetComponent<MeshFilter>().mesh = data.Appearance[status];
        
        if (data.Material.Count != 0)
        {
            GetComponent<MeshRenderer>().materials = data.Material[status];
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
            
        }
    }
}