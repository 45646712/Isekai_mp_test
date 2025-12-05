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

    public void Init(CropBaseData data , int slotID , DateTimeOffset matureTime)
    {
        cropName.text = data.Name;
        cropBackground.sprite = (Sprite)AssetManager.Instance.AllAssets[AssetConstants.AssetType.Sprite].GetAsset(data.DetailBg);
        cropImage.sprite = (Sprite)AssetManager.Instance.AllAssets[AssetConstants.AssetType.Sprite].GetAsset(data.DetailImage);
        
        this.matureTime = matureTime;

        confirmButton.onClick.AddListener(() =>
        {
            if (matureTime.Subtract(DateTimeOffset.Now) <= TimeSpan.Zero)
            {
                CropManager.Instance.Harvest(slotID).Forget();
            }

            Destroy();
        });
        
        cancelButton.onClick.AddListener(() =>
        {
            CropManager.Instance.Remove(slotID).Forget();
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
