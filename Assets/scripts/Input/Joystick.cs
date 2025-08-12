using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class Joystick : OnScreenStick, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform baseArea;
    [SerializeField] private RectTransform anchor;
    [SerializeField] private Image[] images;

    private Vector2 originPos;

    private void Start()
    {
        originPos = baseArea.position;
    }

    public new void OnPointerDown(PointerEventData data)
    {
        anchor.position = data.position;
        UpdateUI(true);

        base.OnPointerDown(data);
    }

    public new void OnPointerUp(PointerEventData data)
    {
        UpdateUI(false);

        base.OnPointerUp(data);
    }

    private void Update()
    {
        baseArea.position = originPos;
    }

    private void UpdateUI(bool isEnable)
    {
        foreach (Image element in images)
        {
            element.enabled = isEnable;
        }
    }
}