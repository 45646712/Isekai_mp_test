using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using static Constant.ItemConstants;
using static Models.ItemModel;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private GameObject ItemSlotPrefab;

    private void Awake()
    {
        Instance = this;
    }
}