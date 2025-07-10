using System;
using System.Collections.Concurrent;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();

    public static void Enqueue(Action action)
    {
        if (action != null)
            _executionQueue.Enqueue(action);
    }

    void Update()
    {
        while (_executionQueue.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }
}