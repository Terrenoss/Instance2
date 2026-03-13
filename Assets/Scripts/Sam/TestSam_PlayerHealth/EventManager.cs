using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    private static EventManager instance;
    

    public event Action PlayerDeath;
    public event Action Victory;
    public event Action PlayerHit;

    private void Awake()
    {
        if (instance) {
            Destroy(this);
        }
        else {
            instance = this;
        }
    }
    
    public void VictoryFunc()
    {
        Victory?.Invoke();
    }

    public void PlayerHitFunc()
    {
        PlayerHit?.Invoke();
    }
}
