using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    
    private void Awake()
    {
        Instance = this;
    }
}
