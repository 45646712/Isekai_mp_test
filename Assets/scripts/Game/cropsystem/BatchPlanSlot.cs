using System;
using AYellowpaper.SerializedCollections;
using Constant;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Utility = Extensions.UniversalUtility;

public class BatchPlanSlot : MonoBehaviour
{
    [SerializeField, SerializedDictionary("crop status", "IconColor")] private SerializedDictionary<CropConstants.CropStatus, Color32> IconColor;

    [SerializeField] private Image Background;
    [SerializeField] private TMP_Text slotID;
    [SerializeField] private TMP_Text cropName;
    [SerializeField] private TMP_Text cropTimer;

    [SerializeField] private float ActivateSize;

    public CropSlot storedSlotData { get; private set; }
    
    private Transform parentAnchor;
    private TimeSpan timeDifference;

    public void Init(CropSlot slot, Transform anchor, int progress)
    {
        parentAnchor = anchor;
        storedSlotData = slot;

        slotID.text = (storedSlotData.slotID + 1).ToString();
        
        if (slot.slotID >= progress)
        {
            GetComponent<Button>().interactable = false;

            Background.color = IconColor[CropConstants.CropStatus.Locked];
            cropName.text = String.Empty;
            cropTimer.text = "Locked";

            enabled = false;
            return;
        }
        
        cropName.text = storedSlotData.data.ID < 1 ? string.Empty : CropConstants.BatchPlanIconIDText + storedSlotData.data.ID;
        Background.color = IconColor[storedSlotData.data.Status];
    }
    
    private void Update()
    {
        //xyz proportional , only needs comparing one axis
        cropName.enabled = parentAnchor.localScale.x > ActivateSize; 
        cropTimer.enabled = parentAnchor.localScale.x > ActivateSize;
        
        Background.color = IconColor[storedSlotData.data.Status];
        
        timeDifference = storedSlotData.data.MatureTime.Subtract(DateTimeOffset.Now);

        if (storedSlotData.data.MatureTime == default)
        {
            cropTimer.text = CropConstants.BatchPlanIconAvailableText;
            return;
        }

        cropTimer.text = timeDifference > TimeSpan.Zero ? Utility.FormatTime(timeDifference) : CropConstants.BatchPlanIconMaturedText;
    }
}
