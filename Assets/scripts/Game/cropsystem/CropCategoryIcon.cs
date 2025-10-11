using System;
using UnityEngine;
using UnityEngine.UI;

public class CropCategoryIcon : MonoBehaviour
{
    private Image icon;
    private Button button;

    private void Awake()
    {
        icon = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void Init(Sprite icon, Action OnclickAction)
    {
        button.onClick.RemoveAllListeners();
        
        this.icon.sprite = icon;
        button.onClick.AddListener(() => OnclickAction?.Invoke());
    }
}
