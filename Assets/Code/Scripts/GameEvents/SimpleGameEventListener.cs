using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleGameEventListener : MonoBehaviour
{
    public SimpleGameEvent gameEvent;

    public UnityEvent unityEvent;


    private void OnEnable()
    {
        gameEvent.AddListerners(this);
    }

    private void OnDisable()
    {
        gameEvent.RemoveListeners(this);
    }


    public void InvokeEvent()
    {
        if (unityEvent != null)
        {
            unityEvent.Invoke();
        }
    }
}
