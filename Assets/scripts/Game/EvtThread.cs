using System;
using System.Collections.Concurrent;
using UnityEngine;
using System.Collections.Generic;

//act as main thread to poll/execute instant void events
[DefaultExecutionOrder(int.MaxValue)]
public class EvtThread : MonoBehaviour
{
    public static event Action QueuedActions; //for instant evt

    private void LateUpdate()
    {
        QueuedActions?.Invoke();
        QueuedActions = null;
    }
}