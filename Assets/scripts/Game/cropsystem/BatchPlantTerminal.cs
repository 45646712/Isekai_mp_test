using System;
using System.Linq;
using Constant;
using UnityEngine;

public class BatchPlantTerminal : MonoBehaviour
{
    [SerializeField] private WorldButton InteractButton;

    private void Start()
    {
        GetComponent<BoxCollider>().enabled = SessionManager.Instance.CurrentSession.IsHost;
    }

    public void Init(CropSlot[] slots)
    {
        InteractButton.Init(() => { UIManager.Instance.SpawnUI(UIConstants.NonPooledUITypes.BatchPlantCropUI).GetComponent<BatchPlantCropUI>().Init(slots); });
    }
    
    private void OnTriggerEnter(Collider col) => InteractButton.UpdateStatus(col.CompareTag("Player"));
    private void OnTriggerExit(Collider col) => InteractButton.UpdateStatus(!col.CompareTag("Player"));
}
