using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEventState", menuName = "GameEvents/GameEventState", order = 100)]
public class GameEventState : ScriptableObject
{
    public List<GameEventListenerState> listeners = new List<GameEventListenerState>();

    public void AddListerners(GameEventListenerState gameListener)
    {
        if (!listeners.Contains(gameListener))
        {
            listeners.Add(gameListener);
        }
    }

    public void RemoveListeners(GameEventListenerState gameListener)
    {
        if (listeners.Contains(gameListener))
        {
            listeners.Remove(gameListener);
        }
    }

    public void Raise(PetCareState selectedState)
    {
        //Debug.Log("Event Raised : "+ selectedState);

        foreach (GameEventListenerState item in listeners)
        {
            if (item != null)
            {
                item.InvokeEvent(selectedState);
            }
        }
    }
}
