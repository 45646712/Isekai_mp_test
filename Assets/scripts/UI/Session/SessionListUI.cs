using System;
using Unity.Services.Multiplayer;
using UnityEngine;

public class SessionListUI : MonoBehaviour
{
    [SerializeField] private GameObject sessionDetailUI;
    [SerializeField] private GameObject listAnchor;

    private async void Start()
    {
        await SessionManager.Instance.GetSessions();
        
        for (int i = 0; i < SessionManager.Instance.allActiveSessions.Count; i++)
        {
            ISessionInfo sessionInfo = SessionManager.Instance.allActiveSessions[i];
            string population = $"{sessionInfo.MaxPlayers - sessionInfo.AvailableSlots} / {sessionInfo.MaxPlayers}";

            SessionDetail element = Instantiate(sessionDetailUI, listAnchor.transform).GetComponent<SessionDetail>();

            element.Init(sessionInfo.Id, population);
        }
    }
}
