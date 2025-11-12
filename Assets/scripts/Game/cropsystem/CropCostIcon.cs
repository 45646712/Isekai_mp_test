using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Constant;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Utility = Extensions.UniversalUtility;

public class CropCostIcon : MonoBehaviour
{
    [SerializeField, SerializedDictionary("ResourceType", "Icon")] private SerializedDictionary<ItemConstants.ResourceType, Sprite> icons;
    
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;

    public void Init(ItemConstants.ResourceType type, int value)
    {
        image.sprite = icons[type];
        text.text = type == ItemConstants.ResourceType.Time ? Utility.FormatTime(value) : value.ToString();
    }
}