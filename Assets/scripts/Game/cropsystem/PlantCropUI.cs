using System;
using System.Collections.Generic;
using System.Linq;
using Constant;
using Cysharp.Threading.Tasks;
using Extensions;
using TMPro;
using Unity.Services.CloudCode.GeneratedBindings.Data;
using UnityEngine;
using UnityEngine.UI;

using static Constant.ItemConstants;
using static Constant.AssetConstants;
using static Models.CropModel;

public class PlantCropUI : MonoBehaviour , IGeneric
{
    [Header("Search")] 
    [SerializeField] private TMP_InputField searchInput;
    [SerializeField] private Button searchButton;
    
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
    private ItemCategory currentCategory;
    private ItemCategory maxCategory;

    private void Awake()
    {
        maxCategory = UniversalUtility.GetEnumMaxIndex(currentCategory);

        previousCategory.onClick.AddListener(() =>
        {
            currentCategory -= 1;
            ShowCategory(currentCategory).Forget();
        });
        nextCategory.onClick.AddListener(() =>
        {
            currentCategory += 1;
            ShowCategory(currentCategory).Forget();
        });

        searchButton.onClick.AddListener(() => { ShowCategory(ItemCategory.All,true).Forget(); });
    }

    private async UniTaskVoid Start()
    {
        RegisterUI();

        await ShowCategory(ItemCategory.All);
    }
    
    private async UniTask ShowCategory(ItemCategory category , bool isSearch = false)
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
        
        previousCategory.GetComponent<Image>().enabled = category > ItemCategory.All;
        nextCategory.GetComponent<Image>().enabled = category < maxCategory;

        categoryText.text = category.ToString();

        List<CropBaseData> result = await CloudCodeManager.Instance.LoadMultiGameData<CropBaseData>(DataConstants_GameDataType.Crop);
        List<CropBaseData> query = result;
        
        if (isSearch)
        {
            query = searchInput.text.All(char.IsDigit) 
                ? result.FindAll(x => x.ID.ToString().Contains(searchInput.text, StringComparison.OrdinalIgnoreCase))
                : result.FindAll(x => x.Name.Contains(searchInput.text, StringComparison.OrdinalIgnoreCase));
        }

        if (category != ItemCategory.All)
        { 
            query = result.Where(x => x.Category == category).ToList();
        }
        
        foreach (CropBaseData element in query)
        {
            CropCategoryIcon icon = PoolManager.Instance.Get(ObjectPoolType.CropIcon, cropIconAnchor).GetComponent<CropCategoryIcon>();
            activeDetailIcons.Add(icon);
            icon.Init(element, () =>
            {
                currentDetailIndex = activeDetailIcons.IndexOf(icon);
                ShowCropDetail(element);
            });

            icon.transform.SetAsLastSibling();
        }

        previousCropDetail.onClick.AddListener(() =>
        {
            currentDetailIndex -= 1;
            ShowCropDetail(activeDetailIcons[currentDetailIndex].baseData);
        });
        nextCropDetail.onClick.AddListener(() =>
        {
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

        previousCropDetail.GetComponent<Image>().enabled = currentDetailIndex > (int)ItemCategory.All;
        nextCropDetail.GetComponent<Image>().enabled = currentDetailIndex < activeDetailIcons.Count - 1;

        displayBackground.sprite = (Sprite)AssetManager.Instance.AllAssets[AssetType.Sprite].GetAsset(data.DetailBg);
        displayImage.sprite = (Sprite)AssetManager.Instance.AllAssets[AssetType.Sprite].GetAsset(data.DetailImage);
        displayName.text = $"No.{data.ID} {data.Name}";
        
        foreach (var (type, value) in data.Costs)
        {
            PoolManager.Instance.Get(ObjectPoolType.CropCostIcon, cropCostIconAnchor).GetComponent<CropCostIcon>().Init(type, value);
        }

        PoolManager.Instance.Get(ObjectPoolType.CropCostIcon, cropCostIconAnchor).GetComponent<CropCostIcon>().InitTime(data.TimeNeeded);
        
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
