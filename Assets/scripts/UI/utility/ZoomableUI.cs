using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class ZoomableUI : MonoBehaviour, IDragHandler
{
    [SerializeField] private Transform target;
    
    [Header("ZoomValues")] 
    [Range(0.5f, 1), SerializeField] private float zoomSpeed = 0.7f;
    [Range(0.5f, 1), SerializeField] private float shrinkThreshold = 0.5f;
    [Range(1, 3), SerializeField] private float zoomThreshold = 2;

    private float zoomValue;

    private void Awake()
    {
        zoomValue = (1 - shrinkThreshold) / (zoomThreshold - shrinkThreshold);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Touch.activeTouches.Count < 2)
        {
            return; //single input , ignore
        }

        Touch touch0 = Touch.activeTouches[0];
        Touch touch1 = Touch.activeTouches[1];
        
        if (touch0.phase != TouchPhase.Moved && touch1.phase != TouchPhase.Moved)
        {
            return;
        }

        if (touch0.history.Count < 1 || touch1.history.Count < 1)
        {
            return;
        }
        
        float ZoomInputValue = Mathf.Clamp(Vector2.Distance(touch0.screenPosition, touch1.screenPosition) - Vector2.Distance(touch0.history[0].screenPosition, touch1.history[0].screenPosition), -1, 1);
        
        zoomValue = ZoomInputValue switch
        {
            < 0 => Mathf.Clamp01(zoomValue -= Time.deltaTime * zoomSpeed),
            > 0 => Mathf.Clamp01(zoomValue += Time.deltaTime * zoomSpeed),
            _ => zoomValue
        };

        target.localScale = Vector3.one * Mathf.Lerp(shrinkThreshold, zoomThreshold, zoomValue);
    }
}
