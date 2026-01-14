using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Constant;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldButton : MonoBehaviour, IWorldUIObject
{
    [field: SerializeField, SerializedDictionary("EnumIndex", "icons")] private SerializedDictionary<int, Material> icons;
    
    private MeshCollider col;
    private new MeshRenderer renderer;

    private Action raycastEvt;
    
    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        col = GetComponent<MeshCollider>();
    }

    public void InvokeRaycastEvt()
    {
        if (!renderer.enabled)
        {
            return;
        }
        
        raycastEvt?.Invoke();
    }

    private void LateUpdate()
    {
        if (renderer.enabled)
        {
            transform.parent.forward = Camera.main.transform.forward;
        }
    }

    public void Init(int index, Action evt) //get enum -> int
    {
        if (index < 0)
        {
            return;
        }

        raycastEvt = evt;
        renderer.material = icons[index];
    }
    
    public void Init(Action evt) => raycastEvt = evt;
    
    public void UpdateStatus(bool isActive) => col.enabled = renderer.enabled = isActive;
}
