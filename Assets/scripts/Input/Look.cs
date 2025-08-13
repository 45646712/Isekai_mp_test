using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class Look : OnScreenControl,IPointerDownHandler, IPointerUpHandler,IDragHandler
{
    [InputControl(layout = "Button"), SerializeField] private string m_ControlPath;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
    
    private Vector2 originPos;

    public void OnPointerDown(PointerEventData data) => SendValueToControl(Vector2.zero);
    public void OnPointerUp(PointerEventData data) => SendValueToControl(Vector2.zero);
    
    public void OnDrag(PointerEventData data)
    {
        SendValueToControl(data.delta.normalized);
    }
}
