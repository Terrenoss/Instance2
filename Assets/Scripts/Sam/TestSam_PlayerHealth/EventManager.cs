using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    private static EventManager instance;
    public static EventManager Instance => instance;

    public event Action PlayerDeath;
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

    public void PlayerDeathFunc()
    {
        PlayerDeath?.Invoke();
    }

    public void PlayerHitFunc()
    {
        PlayerHit?.Invoke();
    }
}
