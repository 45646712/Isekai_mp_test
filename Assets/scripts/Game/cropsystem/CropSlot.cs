using System;
using System.Collections.Generic;
using Constant;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Models;

public class CropSlot : MonoBehaviour
{
    [SerializeField] private WorldButton InteractButton;
    
    public CropModel.CropData data { get; private set; }

    public bool isOccupied { get; private set; }

    private void Start()
    {
        GetComponent<BoxCollider>().enabled = SessionManager.Instance.CurrentSession.IsHost;
        
        SetInteractButton();
    }

    public void Init(CropSO baseData, DateTimeOffset matureTime, CropConstants.CropStatus status = CropConstants.CropStatus.Null)
    {
        data = new(baseData, matureTime, status);
        
        GetComponent<MeshFilter>().mesh = data.Appearance[status];
        
        if (data.Material.Count != 0)
        {
            GetComponent<MeshRenderer>().materials = data.Material[status];
        }

        SetInteractButton();

        isOccupied = true;
    }
    
    public void Reset()
    {
        data = new CropModel.CropData(CropConstants.CropStatus.Null);
        
        GetComponent<MeshFilter>().mesh = null;
        GetComponent<MeshRenderer>().materials = Array.Empty<Material>();

        isOccupied = false;
    }

    private void SetInteractButton()
    {
        switch (data.Status)
        {
            case CropConstants.CropStatus.Null:
                InteractButton.Init(0, () =>
                {
                    if (!UIManager.Instance.AllActiveUIs.ContainsKey(UIConstants.NonPooledUITypes.PlantCropUI))
                    {
                        Instantiate(UIManager.Instance.UIPrefabs[UIConstants.NonPooledUITypes.PlantCropUI]).GetComponent<PlantCropUI>().CurrentSelectedSlot = this;
                    }
                });
                break;
            case CropConstants.CropStatus.Growing:
                InteractButton.Reset();
                break;
            case CropConstants.CropStatus.Matured:
                InteractButton.Init(1, () => { CropManager.Instance.Harvest(CropManager.Instance.AllCrops.IndexOf(this)).Forget(); });
                break;
        }
    }
    
    private void OnTriggerEnter(Collider col) => InteractButton.UpdateStatus(col.CompareTag("Player") && data.Status != CropConstants.CropStatus.Growing);
    private void OnTriggerExit(Collider col) => InteractButton.UpdateStatus(!col.CompareTag("Player") || data.Status == CropConstants.CropStatus.Growing);
}