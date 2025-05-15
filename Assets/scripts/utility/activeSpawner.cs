using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPool : MonoBehaviour
{
    private List<GameObject> spawnedObj = new();
    
    private void Start()
    {
        StartCoroutine(spawn());
        StartCoroutine(Despawn());
    }

    private IEnumerator spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);

            GameObject temp = PoolManager.Instance.AllObjectPools[PoolManager.ObjectPoolType.test].Instance.Get();
            spawnedObj.Add(temp);
            temp.transform.position = new Vector3(0, 5, 0);
        }
    }
    
    private IEnumerator Despawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            
            if (spawnedObj.Count > 0)
            {
                PoolManager.Instance.AllObjectPools[PoolManager.ObjectPoolType.test].Instance.Release(spawnedObj[0]);
                spawnedObj.RemoveAt(0);
            }
        }
    }
}
