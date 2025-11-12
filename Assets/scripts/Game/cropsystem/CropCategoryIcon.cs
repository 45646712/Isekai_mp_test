using System;
using UnityEngine;
using UnityEngine.UI;

public class CropCategoryIcon : MonoBehaviour
{
    public CropSO baseData { get; private set; }
    private Image icon;
    private Button button;

    private void Awake()
    {
        icon = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void Init(CropSO data, Action OnclickAction)
    {
        button.onClick.RemoveAllListeners();

        baseData = data;
        icon.sprite = data.Icon;
        button.onClick.AddListener(() => OnclickAction?.Invoke());
    }
}
