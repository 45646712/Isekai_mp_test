using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Constant;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionFilters : MonoBehaviour
{
    [SerializeField] private SessionListUI mainUI;

    [field: SerializeField, SerializedDictionary("Option", "Privacy")]
    private SerializedDictionary<Toggle, SessionConstants.SessionPrivacy> filterOptions = new();

    [SerializeField] private Toggle hasFriend;
    [SerializeField] private TMP_Dropdown maxQuery;
    [SerializeField] private Button applyButton;
    
    private void Start()
    {
        mainUI = GetComponentInParent<SessionListUI>();
        
        applyButton.onClick.AddListener(ApplyFilter);
    }
    
    private void ApplyFilter()
    {
        mainUI.filterQuery.Clear();
        
        SessionManager.Instance.MaxSessionQuery = int.Parse(maxQuery.options[maxQuery.value].text);
        
        foreach (Toggle element in filterOptions.Keys)
        {
            if (element.isOn)
            {
                mainUI.filterQuery.Add(filterOptions[element]);
            }
        }

        if (hasFriend.isOn)
        {
            mainUI.filterQuery.Add(SessionConstants.SessionPrivacy.Public_friend);
            mainUI.filterQuery.Add(SessionConstants.SessionPrivacy.AskToJoin_friend);
        }

        _ = mainUI.Refresh();
    }
}
