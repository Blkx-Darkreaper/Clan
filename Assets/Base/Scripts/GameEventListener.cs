using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;
    public UnityEvent response;

    protected virtual void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    protected virtual void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }

    public virtual void OnEventRaised()
    {
        response.Invoke();
    }
}