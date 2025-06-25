using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleGameEvent", menuName = "GameEvents/SimpleGameEvent", order = 100)]
public class SimpleGameEvent : ScriptableObject
{
    public List<SimpleGameEventListener> listeners = new List<SimpleGameEventListener>();

    public void AddListerners(SimpleGameEventListener gameListener)
    {
        if (!listeners.Contains(gameListener))
        {
            listeners.Add(gameListener);
        }
    }

    public void RemoveListeners(SimpleGameEventListener gameListener)
    {
        if (listeners.Contains(gameListener))
        {
            listeners.Remove(gameListener);
        }
    }

    public void Raise()
    {
        foreach (SimpleGameEventListener item in listeners)
        {
            if (item != null)
            {
                item.InvokeEvent();
            }
        }
    }
}
