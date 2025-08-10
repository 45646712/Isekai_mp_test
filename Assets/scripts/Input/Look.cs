using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Look : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private void Awake()
    {
        InputManager.Instance.playerInput.Player.Look.Disable();
    }

    public void OnPointerDown(PointerEventData data)=> InputManager.Instance.playerInput.Player.Look.Enable();
    public void OnPointerUp(PointerEventData data) => InputManager.Instance.playerInput.Player.Look.Disable();
}
