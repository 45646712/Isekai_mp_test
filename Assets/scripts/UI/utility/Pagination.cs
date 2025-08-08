using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Constant;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pagination : MonoBehaviour
{
    [SerializeField] private GameObject contentParent;
    [SerializeField] private Scrollbar scrollbar;
    
    [SerializeField] private TMP_Text title;
    [SerializeField] private List<LayoutElement> indicators;

    [field: SerializeField, SerializedDictionary("Index", "Title")]
    private SerializedDictionary<int, string> pageTitle = new();

    private float pageIndex => 1f / contentParent.transform.childCount;

    private Vector2 normalSize = new(60,15);
    private Vector2 selectedSize = new(75,25);

    private void Start()
    {
        scrollbar.onValueChanged.AddListener(UpdatePage);
        
        UpdatePage(0); //initialization
    }

    public void UpdatePage(float value)
    {
        int page = (int)Math.Round(Mathf.Clamp01(value) / pageIndex);

        if (page <= 0)
        {
            page = 1;
        }

        page -= 1;
        
        title.text = pageTitle[page];

        for (int i = 0; i < indicators.Count; i++)
        {
            if (i == page)
            {
                indicators[i].preferredWidth = selectedSize.x;
                indicators[i].preferredHeight = selectedSize.y;
            }
            else
            {
                indicators[i].preferredWidth = normalSize.x;
                indicators[i].preferredHeight = normalSize.y;
            }
        }
    }
}
