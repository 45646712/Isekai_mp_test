using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SessionDetail : MonoBehaviour
{
    [SerializeField] private TMP_Text ID;
    [SerializeField] private TMP_Text population;
    [SerializeField] private Button joinButton;
    
    public void Init(string ID, string population)
    {
        this.ID.text = ID;
        this.population.text = population;
        
        joinButton.onClick.AddListener(async() => { await SessionManager.Instance.StartClient(ID); });
    }
}
