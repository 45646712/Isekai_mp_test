using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Constant;
using Cysharp.Threading.Tasks;
using Extensions;
using Models;
using TMPro;
using Unity.Services.CloudCode.GeneratedBindings.Data;
using UnityEngine;
using UnityEngine.UI;
using static Constant.AssetConstants;
using static Models.CropModel;

using Category = Constant.ItemConstants.ItemCategory;

public class PlantCropUI : MonoBehaviour , IGeneric
{
    [Header("Category")]
    [SerializeField] private Button previousCategory;
    [SerializeField] private Button nextCategory;
    [SerializeField] private TMP_Text categoryText;
    [SerializeField] private Transform cropIconAnchor;
    
    [Header("Detail")]
    [SerializeField] private Button previousCropDetail;
    [SerializeField] private Button nextCropDetail;
    [SerializeField] private Image displayBackground;
    [SerializeField] private Image displayImage;
    [SerializeField] private TMP_Text displayName;
    [SerializeField] private Button detailButton;
    [SerializeField] private Transform cropCostIconAnchor;

    [Header("General")] 
    [SerializeField] private Button CloseButton; 
    [SerializeField] private Button CancelButton;
    [SerializeField] public Button ConfirmButton;

    private List<CropCategoryIcon> activeDetailIcons = new();
    
    public int SlotID { get; set; }

    private int currentDetailIndex;
    private Category currentCategory;
    private Category maxCategory;

    private void Awake()
    {
        maxCategory = UniversalUtility.GetEnumMaxIndex(currentCategory);

        previousCategory.onClick.AddListener(() =>
        {
            if (currentCategory < 0) { return; } //safeguard

            currentCategory -= 1;
            ShowCategory(currentCategory);
        });
        nextCategory.onClick.AddListener(() =>
        {
            if (currentCategory >= maxCategory) { return; } //safeguard

            currentCategory += 1;
            ShowCategory(currentCategory);
        });
    }

    private void Start()
    {
        RegisterUI();

        ShowCategory(0).Forget();
    }

    private async UniTask ShowCategory(Category category)
    {
        currentDetailIndex = 0;
        activeDetailIcons.Clear();
        
        foreach (Transform element in cropIconAnchor.transform)
        {
            if (!element.gameObject.activeInHierarchy)
            {
                continue;
            }

            PoolManager.Instance.Release(ObjectPoolType.CropIcon, element.gameObject);
        }

        previousCropDetail.onClick.RemoveAllListeners();
        nextCropDetail.onClick.RemoveAllListeners();
        
        previousCategory.GetComponent<Image>().enabled = category != 0;
        nextCategory.GetComponent<Image>().enabled = category != maxCategory;

        categoryText.text = (int)category == 0 ? "All" : category.ToString();

        List<CropBaseData> result = await CloudCodeManager.Instance.LoadMultiGameData<CropBaseData>(DataConstants_GameDataType.Crop);

        foreach (CropBaseData element in category == 0 ? result : result.Where(x => x.Category == category))
        {
            CropCategoryIcon icon = PoolManager.Instance.Get(ObjectPoolType.CropIcon, cropIconAnchor).GetComponent<CropCategoryIcon>();
            activeDetailIcons.Add(icon);
            icon.Init(element, () =>
            {
                currentDetailIndex = activeDetailIcons.IndexOf(icon);
                ShowCropDetail(element);
            });
        }
        
        previousCropDetail.onClick.AddListener(() =>
        {
            if (currentDetailIndex < 1) { return; } //safeguard
            
            currentDetailIndex -= 1;
            ShowCropDetail(activeDetailIcons[currentDetailIndex].baseData);
        });
        nextCropDetail.onClick.AddListener(() =>
        {
            if (currentDetailIndex >= activeDetailIcons.Count - 1) { return; } //safeguard

            currentDetailIndex += 1;
            ShowCropDetail(activeDetailIcons[currentDetailIndex].baseData);
        });
        
        if (activeDetailIcons.Count > 0)
        {
            ShowCropDetail(activeDetailIcons[currentDetailIndex].baseData);
        }
    }

    private void ShowCropDetail(CropBaseData data)
    {
        foreach (Transform element in cropCostIconAnchor.transform)
        {
            if (!element.gameObject.activeInHierarchy)
            {
                continue;
            }
            
            PoolManager.Instance.Release(ObjectPoolType.CropCostIcon, element.gameObject);
        }
        
        detailButton.onClick.RemoveAllListeners();

        previousCropDetail.GetComponent<Image>().enabled = currentDetailIndex > 0;
        nextCropDetail.GetComponent<Image>().enabled = currentDetailIndex < activeDetailIcons.Count - 1;

        displayBackground.sprite = (Sprite)AssetManager.Instance.AllAssets[AssetType.Sprite].GetAsset(data.DetailBg);
        displayImage.sprite = (Sprite)AssetManager.Instance.AllAssets[AssetType.Sprite].GetAsset(data.DetailImage);
        displayName.text = $"No.{data.ID} {data.Name}";
        
        foreach (var (type, value) in data.Costs)
        {
            PoolManager.Instance.Get(ObjectPoolType.CropCostIcon, cropCostIconAnchor).GetComponent<CropCostIcon>().Init(type, value);
        }
        
        detailButton.onClick.AddListener(() => { UIManager.Instance.SpawnUI(UIConstants.NonPooledUITypes.CropDetailUI).GetComponent<CropDetailUI>().Init(data.Name, data.Description); }); 
        
        ConfirmButton.onClick.AddListener(() =>
        {
            CropManager.Instance.Plant(SlotID, data.ID).Forget();
            Destroy();
        });
    }
    
    public void RegisterUI() => UIManager.Instance.AllActiveUIs.Add(UIConstants.NonPooledUITypes.PlantCropUI, gameObject);
    public void UnregisterUI() => UIManager.Instance.AllActiveUIs.Remove(UIConstants.NonPooledUITypes.PlantCropUI);

    public void Destroy() => Destroy(gameObject);

    private void OnDestroy()
    {
        PoolManager.Instance.Clear(ObjectPoolType.CropIcon);
        UnregisterUI();
    }
}
