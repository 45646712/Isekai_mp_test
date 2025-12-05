using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Constant;
using Cysharp.Threading.Tasks;
using Extensions;
using TMPro;
using Unity.Services.CloudCode.GeneratedBindings.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Models.CropModel;
using Utility = Extensions.UniversalUtility;
using Category = Constant.ItemConstants.ItemCategory;
using PlanMode = Constant.CropConstants.BatchPlantMode;

public class BatchPlantCropUI : MonoBehaviour, IGeneric
{
    [Header("Category")] 
    [SerializeField] private Button previousCategory;
    [SerializeField] private Button nextCategory;
    [SerializeField] private TMP_Text categoryText;
    [SerializeField] private Transform cropIconAnchor;

    [Header("Detail")] 
    [SerializeField] private ScrollRect BatchPlanMap;
    [SerializeField] private Transform BatchPlanMapAnchor;
    [SerializeField] private Transform cropCostIconAnchor;

    [Header("General")] 
    [SerializeField] private GameObject mapSlotUI;
    [SerializeField] private Button CloseButton;
    [SerializeField] private Button ConfirmButton;
    [SerializeField] private Button CancelButton;
    [SerializeField] private TMP_Text ConfirmButtonText;
    [SerializeField] private TMP_Text CancelButtonText;

    [Header("Utility")]
    [Range(0, 1), SerializeField] private float validClickDuration;
    
    private List<List<BatchPlanSlot>> slots { get; } = new();

    private Category currentCategory;
    private Category maxCategory;

    private CropBaseData currentSelectedBaseData;

    private BatchPlanSlot previousSelectedGrid;
    private BatchPlanSlot currentSelectedGrid;
    
    private float validClickTimer;
    private int validatedClick;
    
    private BatchPlanSlot CurrentSelectedGrid
    {
        get => currentSelectedGrid;
        set
        {
            if (currentSelectedGrid != null)
            {
                previousSelectedGrid = currentSelectedGrid;
            }
            
            currentSelectedGrid = value;
            Plot();
            validatedClick = -1;
        }
    }
    
    private UnityAction OnCancel(PlanMode mode, List<int> selectedSlotIDs) => mode switch
    {
        PlanMode.Standby => () => { CropManager.Instance.Harvest(selectedSlotIDs).Forget(); },
        _ => InitializePlanMap
    };

    private UnityAction OnConfirm(PlanMode mode, List<int> selectedSlotIDs) => mode switch
    {
        PlanMode.Standby => () =>
        {
            List<BatchPlanSlot> validSlots = slots.SelectMany(x => x.Where(y => !y.storedSlotData.isOccupied)).ToList();

            if (validSlots.Count == 0)
            {
                return;
            }

            CancelButtonText.text = UIConstants.GeneralCancelButtonText;
            ConfirmButtonText.text = UIConstants.GeneralConfirmButtonText;

            ShowCost(validSlots);

            CancelButton.onClick.RemoveAllListeners();
            ConfirmButton.onClick.RemoveAllListeners();

            CancelButton.onClick.AddListener(InitializePlanMap);
            ConfirmButton.onClick.AddListener(() =>
            {
                CropManager.Instance.Plant(selectedSlotIDs, currentSelectedBaseData.ID).Forget();
                InitializePlanMap();
            });
        },
        PlanMode.Plant => () => { CropManager.Instance.Plant(selectedSlotIDs, currentSelectedBaseData.ID).Forget(); },
        PlanMode.Remove => () => { CropManager.Instance.Remove(selectedSlotIDs).Forget(); },
        PlanMode.Harvest => () => { CropManager.Instance.Harvest(selectedSlotIDs).Forget(); },
        _ => throw new InvalidOperationException()
    };

    private UnityAction OnInitiatedConfirm(PlanMode mode, BatchPlanSlot slot) => mode switch
    {
        PlanMode.Plant => () => { CropManager.Instance.Plant(slot.SID, currentSelectedBaseData.ID).Forget(); },
        PlanMode.Remove => () =>
        {
            UIManager.Instance.SpawnUI(UIConstants.NonPooledUITypes.CropGrowingUI).GetComponent<CropGrowingUI>().Init(currentSelectedBaseData, slot.SID, slot.storedSlotData.data.MatureTime);
        },
        _ => throw new InvalidOperationException()
    };

    private void Awake()
    {
        maxCategory = UniversalUtility.GetEnumMaxIndex(currentCategory);
        
        previousCategory.onClick.AddListener(() =>
        {
            if (currentCategory < 0)
            {
                return;
            } //safeguard

            currentCategory -= 1;
            ShowCategory(currentCategory);
        });
        nextCategory.onClick.AddListener(() =>
        {
            if (currentCategory >= maxCategory)
            {
                return;
            } //safeguard

            currentCategory += 1;
            ShowCategory(currentCategory);
        });
    }

    private void Start()
    {
        RegisterUI();

        ShowCategory(0);
    }
    
    public void Init(CropSlot[] slots)
    {
        int rows = BatchPlanMapAnchor.GetComponent<GridLayoutGroup>().constraintCount;
        int columns = slots.Length / rows;

        int counter = 0;

        for (int i = 0; i < columns; i++)
        {
            this.slots.Add(new List<BatchPlanSlot>());

            for (int j = 0; j < rows; j++)
            {
                GameObject obj = PoolManager.Instance.Get(ObjectPoolType.BatchMapIcon, BatchPlanMapAnchor);
                Button button = obj.GetComponent<Button>();
                BatchPlanSlot slot = obj.GetComponent<BatchPlanSlot>();

                slot.Init(slots[counter], BatchPlanMapAnchor);

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => { ValidatePlot(slot); });

                this.slots[i].Add(slot);

                counter++;
            }
        }

        InitializePlanMap();
    }

    private void ValidatePlot(BatchPlanSlot slot)
    {
        if (validatedClick != slot.SID)
        {
            validatedClick = slot.SID;
            validClickTimer = validClickDuration;
            return;
        }
        
        CurrentSelectedGrid = slot;
        validClickTimer = 0;
    }
    
    private void Plot()
    {
        CancelButton.onClick.RemoveAllListeners();
        ConfirmButton.onClick.RemoveAllListeners();

        List<BatchPlanSlot> selectedSlots = new();
        List<int> selectedSlotIDs = new();

        CropConstants.CropStatus status;
        PlanMode mode;
        
        switch (previousSelectedGrid == null, currentSelectedGrid == null)
        {
            case (true, true):
                InitializePlanMap();
                break;
            case (true, false):
                status = currentSelectedGrid.storedSlotData.data.Status;
                mode = this.GetPlanMode(status);
                
                CancelButtonText.text = CropConstants.BatchPlanCancelText[mode];
                ConfirmButtonText.text = CropConstants.BatchPlanInitConfirmText[mode];

                CancelButton.onClick.AddListener(OnCancel(mode, null));

                if (mode == PlanMode.Harvest)
                {
                    ConfirmButton.onClick.AddListener(OnConfirm(mode, slots.SelectMany(x => x.Select(y => y.SID)).ToList()) + InitializePlanMap);
                }
                else
                {
                    ConfirmButton.onClick.AddListener(OnInitiatedConfirm(mode, currentSelectedGrid) + InitializePlanMap);
                }
                
                if (mode == PlanMode.Plant)
                {
                    ShowCost();
                }
                break;
            default:
                status = previousSelectedGrid.storedSlotData.data.Status;
                mode = this.GetPlanMode(status);

                (int,int) g1 = Utility.IndexOf2LayerList(slots, previousSelectedGrid);
                (int,int) g2 = Utility.IndexOf2LayerList(slots, currentSelectedGrid);

                (int, int) d1 = (Math.Min(g1.Item1, g2.Item1), Math.Min(g1.Item2, g2.Item2)); //d1.x , d1.y
                (int, int) d2 = (Math.Max(g1.Item1, g2.Item1), Math.Max(g1.Item2, g2.Item2)); //d2.x , d2,y

                for (int x = d1.Item1; x <= d2.Item1; x++)
                {
                    for (int y = d1.Item2; y <= d2.Item2; y++)
                    {
                        selectedSlots.Add(slots[x][y]);
                    }
                }

                selectedSlotIDs = mode != PlanMode.Harvest
                    ? selectedSlots.Select(x => x.SID).ToList()
                    : slots.SelectMany(x => x.Select(y => y.SID)).ToList();

                if (status == CropConstants.CropStatus.Null)
                {
                    ShowCost(selectedSlots);
                }
                
                CancelButtonText.text = CropConstants.BatchPlanCancelText[mode];
                ConfirmButtonText.text = CropConstants.BatchPlanConfirmText[mode];
                
                CancelButton.onClick.AddListener(OnCancel(mode, selectedSlotIDs) + InitializePlanMap);
                ConfirmButton.onClick.AddListener(OnConfirm(mode, selectedSlotIDs) + InitializePlanMap);
                
                previousSelectedGrid = null;
                currentSelectedGrid = null;
                break;
        }
    }
    
    private void Update()
    {
        //double click checking
        if (validClickTimer > 0)
        {
            validClickTimer -= Time.deltaTime;
        }
        else if (validatedClick != -1 && validClickTimer <= 0)
        {
            validatedClick = -1;
        }
    }

    private async void ShowCategory(Category category)
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

        List<CropBaseData> result = await CloudCodeManager.Instance.LoadMultiGameData<CropBaseData>(DataConstants_GameDataType.Crop);
        
        foreach (CropBaseData element in category == 0 ? result : result.Where(x => x.Category == category))
        {
            PoolManager.Instance.Get(ObjectPoolType.CropIcon, cropIconAnchor).GetComponent<CropCategoryIcon>().Init(element, () => { currentSelectedBaseData = element; });
        }

        if (currentSelectedBaseData == null)
        {
            currentSelectedBaseData = result.FirstOrDefault();
        }
    }

    private void ShowCost() 
    {
        foreach (Transform element in cropCostIconAnchor)
        {
            PoolManager.Instance.Release(ObjectPoolType.CropCostIcon, element.gameObject);
        }
        
        foreach (var (type, value) in currentSelectedBaseData.Costs)
        {
            PoolManager.Instance.Get(ObjectPoolType.CropCostIcon, cropCostIconAnchor).GetComponent<CropCostIcon>().Init(type, value);
        }
    }
    
    private void ShowCost(List<BatchPlanSlot> selectedSlots) 
    {
        Dictionary<ItemConstants.ResourceType, int> costs = new();

        int totalValidSlots = selectedSlots.Count(x => !x.storedSlotData.isOccupied);
        
        foreach (var (type, value) in currentSelectedBaseData.Costs)
        {
            costs[type] = type switch
            {
                ItemConstants.ResourceType.Time => value,
                _ => costs.TryGetValue(type, out var cost) ? cost += value * totalValidSlots : value * totalValidSlots
            };
        }

        foreach (Transform element in cropCostIconAnchor)
        {
            PoolManager.Instance.Release(ObjectPoolType.CropCostIcon, element.gameObject);
        }
        
        foreach (var (type, value) in costs)
        {
            PoolManager.Instance.Get(ObjectPoolType.CropCostIcon, cropCostIconAnchor).GetComponent<CropCostIcon>().Init(type, value);
        }
    }

    private void InitializePlanMap()
    {
        List<int> selectedSlotIDs = slots.SelectMany(x => x.Select(y => y.SID)).ToList();

        CancelButton.onClick.RemoveAllListeners();
        ConfirmButton.onClick.RemoveAllListeners();

        CancelButtonText.text = CropConstants.BatchPlanCancelText[PlanMode.Standby];
        ConfirmButtonText.text = CropConstants.BatchPlanConfirmText[PlanMode.Standby];

        CancelButton.onClick.AddListener(OnCancel(PlanMode.Standby, selectedSlotIDs) + InitializePlanMap);
        ConfirmButton.onClick.AddListener(OnConfirm(PlanMode.Standby, selectedSlotIDs));
        
        foreach (Transform element in cropCostIconAnchor)
        {
            PoolManager.Instance.Release(ObjectPoolType.CropCostIcon, element.gameObject);
        }
        
        previousSelectedGrid = null;
        currentSelectedGrid = null;
    }

    public void RegisterUI() => UIManager.Instance.AllActiveUIs.Add(UIConstants.NonPooledUITypes.BatchPlantCropUI, gameObject);
    public void UnregisterUI() => UIManager.Instance.AllActiveUIs.Remove(UIConstants.NonPooledUITypes.BatchPlantCropUI);

    public void Destroy() => Destroy(gameObject);

    private void OnDestroy()
    {
        PoolManager.Instance.Clear(ObjectPoolType.CropIcon);
        PoolManager.Instance.Clear(ObjectPoolType.CropCostIcon);
        PoolManager.Instance.Clear(ObjectPoolType.BatchMapIcon);
        
        UnregisterUI();
    }
}
