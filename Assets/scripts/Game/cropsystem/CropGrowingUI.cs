using System;
using Constant;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static Models.CropModel;
using static Extensions.UniversalUtility;

public class CropGrowingUI : MonoBehaviour, IGeneric
{
    [SerializeField] private TMP_Text cropName;
    [SerializeField] private Image cropBackground;
    [SerializeField] private Image cropImage;
    [SerializeField] private TMP_Text cropTimer;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text cancelButtonText;
    [SerializeField] private TMP_Text confirmButtonText;

    private DateTimeOffset matureTime;
    private TimeSpan timeDifference;
    
    private void Start()
    {
        RegisterUI();
    }

    public void Init(CropSlot slot)
    {
        cropName.text = slot.data.Name;
        cropBackground.sprite = slot.data.DetailBg;
        cropImage.sprite = slot.data.DetailImage;

        matureTime = slot.data.MatureTime;

        confirmButton.onClick.AddListener(() =>
        {
            if (matureTime.Subtract(DateTimeOffset.Now) <= TimeSpan.Zero)
            {
                CropManager.Instance.Harvest(slot.slotID).Forget();
            }

            Destroy();
        });
        
        cancelButton.onClick.AddListener(() =>
        {
            CropManager.Instance.Remove(slot.slotID).Forget();
            Destroy();
        });
    }

    private void Update()
    {
        timeDifference = matureTime.Subtract(DateTimeOffset.Now);
        bool isGrowing = timeDifference > TimeSpan.Zero;
        
        cancelButton.gameObject.SetActive(isGrowing);
        cropTimer.text = isGrowing ? $"{CropConstants.CropAwaitText}{FormatTime(timeDifference)}" : $"{CropConstants.CropMaturedText}";
        confirmButtonText.text = isGrowing ? "Confirm" : "Harvest";
    }

    public void RegisterUI() => UIManager.Instance.AllActiveUIs.Add(UIConstants.NonPooledUITypes.CropGrowingUI, gameObject);
    public void UnregisterUI() => UIManager.Instance.AllActiveUIs.Remove(UIConstants.NonPooledUITypes.CropGrowingUI);

    public void Destroy() => Destroy(gameObject);

    private void OnDestroy()
    {
        UnregisterUI();
    }
}
