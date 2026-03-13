using System;
using UnityEngine;

public class CheckVictory : MonoBehaviour
{
    public event Action OnVictory;

    private void OnTriggerEnter (Collider coll)
    {
        if (coll.gameObject.TryGetComponent(out PlayerHealth playerHealth))
        {
            OnVictory?.Invoke();
        }
    }
}
