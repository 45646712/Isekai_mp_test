using System;
using Constant;
using Cysharp.Threading.Tasks;
using Extensions;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MySession : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown privacyOption;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private Button applyButton;
    [SerializeField] private TMP_Text buttonText;
    
    private void Start()
    {
        privacyOption.onValueChanged.AddListener(value =>
        {
            password.gameObject.SetActive(value == (int)SessionConstants.SessionPrivacy.Private);
        });
        
        applyButton.onClick.AddListener(()=>
        {
            if (NetworkManager.Singleton.IsHost)
            {
                SessionManager.Instance.CurrentSession.SetPrivacyState((SessionConstants.SessionPrivacy)privacyOption.value, password.text);
            }
            else
            {
                SessionManager.Instance.StartHost((SessionConstants.SessionPrivacy)privacyOption.value, password.text).Forget();
            }
        });
        
        if (!NetworkManager.Singleton.IsHost)
        {
            buttonText.text = "Return";
        }
    }
}
