using System;
using UnityEngine;

/// <summary>
/// condition:
/// 1. height must be same (absolute flat terrain)
/// 2. every outer bound and every inner bound must be same
/// 3. transform vector value must be positive count
/// will maintain equal distance between each other  
/// </summary>
public class AutoGeneration : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private Transform Anchor;
    
    [SerializeField] private int horizontalCount;
    [SerializeField] private int verticalCount;
    [SerializeField] private float innerBound;
    [SerializeField] private float outerBound;

    private Vector3 spawnOrigin;
    private Vector2 sourceBound;
    
    private void Awake()
    {
        spawnOrigin = Anchor.position;
        Destroy(Anchor.gameObject);
        
        BoxCollider col = Instantiate(prefab).GetComponent<BoxCollider>();
        sourceBound = new Vector2(col.bounds.extents.x * 2, col.bounds.extents.z * 2);
        Destroy(col.gameObject);

        spawnOrigin = new Vector3(spawnOrigin.x + outerBound, spawnOrigin.y, spawnOrigin.z + outerBound); //in localspace

        Vector3 defaultOrigin = spawnOrigin;
        int counter = 1;

        for (int i = 0; i < verticalCount; i++)
        {
            for (int j = 0; j < horizontalCount; j++)
            {
                spawnOrigin = new Vector3(defaultOrigin.x + sourceBound.x + innerBound * j, defaultOrigin.y, defaultOrigin.z + sourceBound.y + innerBound * i);
                GameObject obj = Instantiate(prefab, gameObject.transform, worldPositionStays: true);
                
                obj.name = $"Crop{counter}";
                obj.transform.position = spawnOrigin;
                
                counter++;
            }

            spawnOrigin = new Vector3(defaultOrigin.x, defaultOrigin.y, defaultOrigin.z);
        }

        Debug.Log($"total width(x) : {outerBound * 2 + sourceBound.x * 2 + innerBound * (horizontalCount - 1)}");
        Debug.Log($"total length(z) : {outerBound * 2 + sourceBound.y * 2 + innerBound * (verticalCount - 1)}");
    }
}
