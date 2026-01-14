using System;
using System.Linq;
using Constant;
using UnityEngine;

public class BatchPlantTerminal : MonoBehaviour
{
    [SerializeField] private WorldButton InteractButton;
    
    public void Init(CropSlot[] slots)
    {
        GetComponent<BoxCollider>().enabled = true;
        
        InteractButton.Init(() => { UIManager.Instance.SpawnUI(UIConstants.NonPooledUITypes.BatchPlantCropUI).GetComponent<BatchPlantCropUI>().Init(slots).Forget(); });
    }
    
    private void OnTriggerEnter(Collider col) => InteractButton.UpdateStatus(col.CompareTag("Player"));
    private void OnTriggerExit(Collider col) => InteractButton.UpdateStatus(!col.CompareTag("Player"));
}
