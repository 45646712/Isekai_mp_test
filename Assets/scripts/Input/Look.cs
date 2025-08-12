using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class Look : OnScreenStick
{
    public new void OnPointerDown(PointerEventData data)
    {
       // anchor.position = data.position;
       Debug.Log(InputManager.Instance.playerInput.Player.Look.ReadValue<Vector2>());
        base.OnPointerDown(data);
    }
    public new void OnDrag(PointerEventData data)
    {
        // anchor.position = data.position;
        Debug.Log(InputManager.Instance.playerInput.Player.Look.ReadValue<Vector2>());
        base.OnDrag(data);
    }
    public new void OnPointerUp(PointerEventData data)
    {
        // anchor.position = data.position;
        Debug.Log(InputManager.Instance.playerInput.Player.Look.ReadValue<Vector2>());
        base.OnPointerUp(data);
    }

    private void Update()
    {
        //baseArea.position = originPos;
    }
}
