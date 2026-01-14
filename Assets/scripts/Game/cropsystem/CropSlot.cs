using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Constant;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using static Models.CropModel;

public class CropSlot : NetworkBehaviour
{
    [SerializeField] private Transform InteractButtonAnchor;

    [SerializeField] private Mesh LockedMesh;
    [SerializeField] private Material[] LockedMaterial;

    private GameObject interactButtonObj;
    private WorldButton interactButton;
    
    public CropData data { get; private set; } = new();
    public int slotID { get; set; }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public override void OnNetworkSpawn()
    {
        if (NetworkManager.IsHost)
        {
            interactButtonObj = PoolManager.Instance.Get(ObjectPoolType.CropButton, NonNetworkEntityParent.instance.EntityParents[UIConstants.NonNetworkEntityTypes.CropButton], InteractButtonAnchor.position);
            interactButton = interactButtonObj.GetComponentInChildren<WorldButton>();

            SetInteractButton();
        }
        else
        {
            Destroy(interactButtonObj); // client will not register pool , destroy directly
        }
    }

    [Rpc(SendTo.Everyone)] // host -> self + clients
    public void LockRpc()
    {
        data.Lock();

        GetComponent<MeshFilter>().mesh = LockedMesh;
        GetComponent<MeshRenderer>().materials = LockedMaterial;

        if (NetworkManager.IsHost)
        {
            interactButton.gameObject.SetActive(false);
        }
    }

    [Rpc(SendTo.Everyone)] // host -> self + clients
    public void UnlockRpc()
    {
        data.UnLock();

        if (NetworkManager.IsHost)
        {
            interactButton.gameObject.SetActive(true);
        }
    }

    [Rpc(SendTo.Everyone)] // host -> self + clients
    public void InitRpc(string rawBaseData, string matureTime = null, CropConstants.CropStatus status = CropConstants.CropStatus.Null)
    {
        CropBaseData baseData = JsonSerializer.Deserialize<CropBaseData>(rawBaseData);

        data = string.IsNullOrEmpty(matureTime)
            ? new(baseData, DateTimeOffset.UtcNow.AddSeconds(baseData.TimeNeeded), status)
            : new(baseData, DateTimeOffset.Parse(matureTime), status);

        GetComponent<MeshFilter>().mesh = data.Appearance[status];

        if (data.Material[status].Length != 0)
        {
            GetComponent<MeshRenderer>().materials = data.Material[status];
        }
        
        if (NetworkManager.IsHost)
        {
            SetInteractButton();
        }
    }
    
    [Rpc(SendTo.Everyone)] // host -> self + clients
    public void ResetRpc()
    {
        data = new CropData();

        GetComponent<MeshFilter>().mesh = null;

        if (NetworkManager.IsHost)
        {
            SetInteractButton();
        }
    }
    
    private void SetInteractButton()
    {
        switch (data.Status)
        {
            case CropConstants.CropStatus.Null:
                interactButton.Init((int)data.Status, () => { UIManager.Instance.SpawnUI(UIConstants.NonPooledUITypes.PlantCropUI).GetComponent<PlantCropUI>().SlotID = slotID; });
                break;
            case CropConstants.CropStatus.Growing:
                interactButton.Init((int)data.Status, () => { UIManager.Instance.SpawnUI(UIConstants.NonPooledUITypes.CropGrowingUI).GetComponent<CropGrowingUI>().Init(this); });
                break;
            case CropConstants.CropStatus.Matured:
                interactButton.Init((int)data.Status, () => { CropManager.Instance.Harvest(slotID).Forget(); });
                break;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (NetworkManager.IsHost)
        {
            interactButton.UpdateStatus(col.CompareTag("Player"));
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (NetworkManager.IsHost)
        {
            interactButton.UpdateStatus(!col.CompareTag("Player"));
        }
    }

    public override void OnNetworkDespawn() => PoolManager.Instance.Clear(ObjectPoolType.CropButton);
}