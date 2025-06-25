using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerState : MonoBehaviour
{
    public GameEventState gameEvent;

    public UnityEvent<PetCareState> unityEvent;


    private void OnEnable()
    {
        gameEvent.AddListerners(this);
    }

    private void OnDisable()
    {
        gameEvent.RemoveListeners(this);
    }


    public void InvokeEvent(PetCareState state)
    {
        if (unityEvent != null)
        {
            unityEvent.Invoke(state);
        }
    }
}
