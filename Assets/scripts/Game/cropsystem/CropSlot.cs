using System;
using System.Collections.Generic;
using System.Linq;
using Constant;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Models;

public class CropSlot : MonoBehaviour
{
    [SerializeField] private WorldButton InteractButton;
    
    public CropModel.CropData data { get; private set; }
    public bool isOccupied { get; private set; }

    private int slotID;
    
    private void Start()
    {
        GetComponent<BoxCollider>().enabled = SessionManager.Instance.CurrentSession.IsHost;
        
        slotID = CropManager.Instance.AllCrops.IndexOf(this);
        
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

        SetInteractButton();
        
        isOccupied = false;
    }

    private void SetInteractButton()
    {
        switch (data.Status)
        {
            case CropConstants.CropStatus.Null:
                InteractButton.Init((int)data.Status, () => { UIManager.Instance.SpawnUI(UIConstants.NonPooledUITypes.PlantCropUI).GetComponent<PlantCropUI>().SlotID = slotID; });
                break;
            case CropConstants.CropStatus.Growing:
                InteractButton.Init((int)data.Status, () =>
                {
                    CropSO baseData = Array.Find(CropManager.Instance.AllCropBaseData, x => x.ID == data.ID);
                    UIManager.Instance.SpawnUI(UIConstants.NonPooledUITypes.CropGrowingUI).GetComponent<CropGrowingUI>().Init(baseData, slotID, data.MatureTime);
                });
                break;
            case CropConstants.CropStatus.Matured:
                InteractButton.Init((int)data.Status, () =>
                {
                    CropManager.Instance.Harvest(CropManager.Instance.AllCrops.IndexOf(this)).Forget();
                });
                break;
        }
    }
    
    private void OnTriggerEnter(Collider col) => InteractButton.UpdateStatus(col.CompareTag("Player"));
    private void OnTriggerExit(Collider col) => InteractButton.UpdateStatus(!col.CompareTag("Player"));
}