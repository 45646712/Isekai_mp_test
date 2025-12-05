using System;
using Constant;
using UnityEngine;
using UnityEngine.UI;

using static Models.CropModel;

public class CropCategoryIcon : MonoBehaviour
{
    public CropBaseData baseData { get; private set; }
    private Image icon;
    private Button button;

    private void Awake()
    {
        icon = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void Init(CropBaseData data, Action OnclickAction)
    {
        button.onClick.RemoveAllListeners();

        baseData = data;
        icon.sprite = (Sprite)AssetManager.Instance.AllAssets[AssetConstants.AssetType.Sprite].GetAsset(data.Icon);
        button.onClick.AddListener(() => OnclickAction?.Invoke());
    }
}
