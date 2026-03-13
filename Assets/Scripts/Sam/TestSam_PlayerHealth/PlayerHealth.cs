using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 3;
    
    [SerializeField] private ScoreManager scoreManager;

    public event Action OnPlayerHit;
    public event Action OnPlayerDeath;

    public void TakeDamage()
    {
        health--;
        scoreManager.DecreaseMultiplier();
        
        OnPlayerHit?.Invoke();
        
        if (health <= 0 ) {
            Death();
        }
    }

    public void Death()
    {
        OnPlayerDeath?.Invoke();
        gameObject.SetActive(false);
    }
}
