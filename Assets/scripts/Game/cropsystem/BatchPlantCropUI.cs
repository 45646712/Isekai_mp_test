using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Constant;
using Cysharp.Threading.Tasks;
using Extensions;
using TMPro;
using Unity.Services.CloudCode.GeneratedBindings.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using static Constant.DataConstants;
using static Constant.ItemConstants;
using static Constant.CropConstants;
using static Models.CropModel;
using static Extensions.UniversalUtility;

public class BatchPlantCropUI : MonoBehaviour, IGeneric
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

    private ItemCategory currentCategory;
    private ItemCategory maxCategory;

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

    private CropBaseData CurrentSelectedBaseData
    {
        get => currentSelectedBaseData;
        set
        {
            InitializePlanMap();
            currentSelectedBaseData = value;
        }
    }
    
    private UnityAction OnCancel(BatchPlantMode mode) => mode switch
    {
        BatchPlantMode.Standby => () => { CropManager.Instance.Harvest(slots.SelectMany(x => x.Where(y => y.storedSlotData.data.Status == CropStatus.Matured).Select(y => y.storedSlotData.slotID)).ToList()).Forget(); },
        _ => InitializePlanMap
    };

    private UnityAction OnConfirm(BatchPlantMode mode, List<int> selectedSlotIDs) => mode switch
    {
        BatchPlantMode.Standby => () =>
        {
            List<BatchPlanSlot> validSlots = slots.SelectMany(x => x.Where(y => y.storedSlotData.data.Status == CropStatus.Null)).ToList();

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
                CropManager.Instance.Plant(validSlots.Select(x => x.storedSlotData.slotID).ToList(), CurrentSelectedBaseData.ID).Forget();
                InitializePlanMap();
            });
        },
        BatchPlantMode.Plant => () => { CropManager.Instance.Plant(selectedSlotIDs, CurrentSelectedBaseData.ID).Forget(); },
        BatchPlantMode.Remove => () => { CropManager.Instance.Remove(selectedSlotIDs).Forget(); },
        BatchPlantMode.Harvest => () => { CropManager.Instance.Harvest(selectedSlotIDs).Forget(); },
        _ => throw new InvalidOperationException()
    };

    private UnityAction OnInitiatedConfirm(BatchPlantMode mode, BatchPlanSlot slot) => mode switch
    {
        BatchPlantMode.Plant => () => { CropManager.Instance.Plant(slot.storedSlotData.slotID, CurrentSelectedBaseData.ID).Forget(); }, 
        BatchPlantMode.Remove => () => { UIManager.Instance.SpawnUI(UIConstants.NonPooledUITypes.CropGrowingUI).GetComponent<CropGrowingUI>().Init(slot.storedSlotData); },
        _ => throw new InvalidOperationException()
    };

    private void Awake()
    {
        maxCategory = GetEnumMaxIndex(currentCategory);
        
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

        searchButton.onClick.AddListener(() => { ShowCategory(ItemCategory.All, true).Forget(); });
    }

    private async UniTaskVoid Start()
    {
        RegisterUI();
        
        await ShowCategory(ItemCategory.All);
    }
    
    public async UniTaskVoid Init(CropSlot[] slots)
    {
        int result = (int)await CloudCodeManager.Instance.LoadPlayerData(DataConstants_DataAccessibility.Protected, nameof(ProtectedDataType.UnlockedCropSlots));
        
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

                slot.Init(slots[counter], BatchPlanMapAnchor, result);

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
        if (validatedClick != slot.storedSlotData.slotID)
        {
            validatedClick = slot.storedSlotData.slotID;
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

        CropStatus status;
        BatchPlantMode mode;
        
        switch (previousSelectedGrid == null, currentSelectedGrid == null)
        {
            case (true, true):
                InitializePlanMap();
                break;
            case (true, false):
                status = currentSelectedGrid.storedSlotData.data.Status;
                mode = this.GetPlanMode(status);
                
                CancelButtonText.text = BatchPlanCancelText[mode];
                ConfirmButtonText.text = BatchPlanInitConfirmText[mode];

                CancelButton.onClick.AddListener(OnCancel(mode));

                if (mode == BatchPlantMode.Harvest)
                {
                    ConfirmButton.onClick.AddListener(OnConfirm(mode, slots.SelectMany(x => x.Where(y => y.storedSlotData.data.Status == CropStatus.Matured).Select(y => y.storedSlotData.slotID)).ToList()) + InitializePlanMap);
                }
                else
                {
                    ConfirmButton.onClick.AddListener(OnInitiatedConfirm(mode, currentSelectedGrid) + InitializePlanMap);
                }
                
                if (mode == BatchPlantMode.Plant)
                {
                    ShowCost();
                }
                break;
            default:
                status = previousSelectedGrid.storedSlotData.data.Status;
                mode = this.GetPlanMode(status);

                (int,int) g1 = IndexOf2LayerList(slots, previousSelectedGrid);
                (int,int) g2 = IndexOf2LayerList(slots, currentSelectedGrid);

                (int, int) d1 = (Math.Min(g1.Item1, g2.Item1), Math.Min(g1.Item2, g2.Item2)); //d1.x , d1.y
                (int, int) d2 = (Math.Max(g1.Item1, g2.Item1), Math.Max(g1.Item2, g2.Item2)); //d2.x , d2,y

                for (int x = d1.Item1; x <= d2.Item1; x++)
                {
                    for (int y = d1.Item2; y <= d2.Item2; y++)
                    {
                        selectedSlots.Add(slots[x][y]);
                    }
                }

                selectedSlotIDs = mode switch
                {
                    BatchPlantMode.Plant => selectedSlots.Where(x => x.storedSlotData.data.Status == CropStatus.Null).Select(x => x.storedSlotData.slotID).ToList(),
                    BatchPlantMode.Harvest => slots.SelectMany(x => x.Where(y => y.storedSlotData.data.Status == CropStatus.Matured).Select(y => y.storedSlotData.slotID)).ToList(),
                    BatchPlantMode.Remove => selectedSlots.Select(x => x.storedSlotData.slotID).ToList(),
                    _ => throw new InvalidOperationException()
                };

                if (status == CropStatus.Null)
                {
                    ShowCost(selectedSlots);
                }
                
                CancelButtonText.text = BatchPlanCancelText[mode];
                ConfirmButtonText.text = BatchPlanConfirmText[mode];
                
                CancelButton.onClick.AddListener(OnCancel(mode) + InitializePlanMap);
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

    private async UniTask ShowCategory(ItemCategory category, bool isSearch = false)
    {
        foreach (Transform element in cropIconAnchor.transform)
        {
            if (!element.gameObject.activeInHierarchy)
            {
                continue;
            }

            PoolManager.Instance.Release(ObjectPoolType.CropIcon, element.gameObject);
        }

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
        
        InitializePlanMap();
        
        foreach (CropBaseData element in query)
        {
            GameObject icon = PoolManager.Instance.Get(ObjectPoolType.CropIcon, cropIconAnchor);
            icon.GetComponent<CropCategoryIcon>().Init(element, () => { CurrentSelectedBaseData = element; });
            icon.transform.SetAsLastSibling();
        }

        CurrentSelectedBaseData ??= query.FirstOrDefault(); // =to apply directly or ??= to not apply
    }

    private void ShowCost() 
    {
        foreach (Transform element in cropCostIconAnchor)
        {
            PoolManager.Instance.Release(ObjectPoolType.CropCostIcon, element.gameObject);
        }
        
        foreach (var (type, value) in CurrentSelectedBaseData.Costs)
        {
            PoolManager.Instance.Get(ObjectPoolType.CropCostIcon, cropCostIconAnchor).GetComponent<CropCostIcon>().Init(type, value);
        }
        
        PoolManager.Instance.Get(ObjectPoolType.CropCostIcon, cropCostIconAnchor).GetComponent<CropCostIcon>().InitTime(CurrentSelectedBaseData.TimeNeeded);
    }
    
    private void ShowCost(List<BatchPlanSlot> selectedSlots) 
    {
        Dictionary<ResourceType, int> costs = new();

        int totalValidSlots = selectedSlots.Count(x => x.storedSlotData.data.Status == CropStatus.Null);
        
        foreach (var (type, value) in CurrentSelectedBaseData.Costs)
        {
            costs[type] = costs.TryGetValue(type, out var cost) ? cost + value * totalValidSlots : value * totalValidSlots;
        }
        
        foreach (Transform element in cropCostIconAnchor)
        {
            PoolManager.Instance.Release(ObjectPoolType.CropCostIcon, element.gameObject);
        }
        
        foreach (var (type, value) in costs)
        {
            PoolManager.Instance.Get(ObjectPoolType.CropCostIcon, cropCostIconAnchor).GetComponent<CropCostIcon>().Init(type, value);
        }

        PoolManager.Instance.Get(ObjectPoolType.CropCostIcon, cropCostIconAnchor).GetComponent<CropCostIcon>().InitTime(CurrentSelectedBaseData.TimeNeeded);
    }

    private void InitializePlanMap()
    {
        CancelButton.onClick.RemoveAllListeners();
        ConfirmButton.onClick.RemoveAllListeners();

        CancelButtonText.text = BatchPlanCancelText[BatchPlantMode.Standby];
        ConfirmButtonText.text = BatchPlanConfirmText[BatchPlantMode.Standby];

        CancelButton.onClick.AddListener(OnCancel(BatchPlantMode.Standby) + InitializePlanMap);
        ConfirmButton.onClick.AddListener(OnConfirm(BatchPlantMode.Standby, null));
        
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
