using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Constant;
using Cysharp.Threading.Tasks;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public CropSlot CurrentSelectedSlot { get; set; }

    private Category currentCategory;
    private Category maxCategory;
    
    private int CurrentSelectedCropID;
    
    private void Awake()
    {
        maxCategory = UniversalUtility.GetEnumMaxIndex(currentCategory);

        previousCategory.onClick.AddListener(() =>
        {
            if (currentCategory < 0) //safeguard
            {
                currentCategory = 0;
                return;
            }

            currentCategory -= 1;
            ShowCategory(currentCategory);
        });
        nextCategory.onClick.AddListener(() =>
        {
            if (currentCategory >= maxCategory) //safeguard
            {
                currentCategory = maxCategory;
                return; 
            }

            currentCategory += 1;
            ShowCategory(currentCategory);
        });
    }

    private void Start()
    {
        RegisterUI();

        ShowCategory(0);
        InitCropDetail(CropManager.Instance.AllCropBaseData.First());
    }

    private void ShowCategory(Category category)
    {
        foreach (Transform element in cropIconAnchor.transform)
        {
            if (!element.gameObject.activeInHierarchy)
            {
                continue;
            }
            
            PoolManager.Instance.Release(ObjectPoolType.CropIcon, element.gameObject);
        }
        
        previousCategory.GetComponent<Image>().enabled = category != 0;
        nextCategory.GetComponent<Image>().enabled = category != maxCategory;

        categoryText.text = (int)category == 0 ? "All" : category.ToString();

        foreach (CropSO element in CropManager.Instance.AllCropBaseData.Where(x => category == 0 ? x : x.Category == category))
        {
            PoolManager.Instance.Get(ObjectPoolType.CropIcon, cropIconAnchor).GetComponent<CropCategoryIcon>().Init(element.Icon, () => { InitCropDetail(element); });
        }
    }

    private void InitCropDetail(CropSO data)
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

        CurrentSelectedCropID = data.ID;

        displayBackground.sprite = data.DetailBg;
        displayImage.sprite = data.DetailImage;
        displayName.text = $"No.{data.ID} {data.Name}";
        
        foreach (var (type, value) in data.Costs)
        {
            PoolManager.Instance.Get(ObjectPoolType.CropCostIcon, cropCostIconAnchor).GetComponent<CropCostIcon>().Init(type, value);
        }
        
        detailButton.onClick.AddListener(() => { }); // show detail -> basedata.description
        ConfirmButton.onClick.AddListener(() =>
        {
            CropManager.Instance.Plant(CurrentSelectedSlot, data).Forget();
            Destroy();
        });
    }
    
    public void RegisterUI() => UIManager.Instance.AllActiveUIs.Add(UIConstants.NonPooledUITypes.PlantCropUI, gameObject);
    public void UnregisterUI() => UIManager.Instance.AllActiveUIs.Remove(UIConstants.NonPooledUITypes.PlantCropUI);

    public void Destroy() => Destroy(gameObject);

    private void OnDestroy()
    {
        UnregisterUI();
    }
}
