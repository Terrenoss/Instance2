using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 3;
    
    [SerializeField] private ScoreManager scoreManager;
    
    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    private bool isInvincible = false;

    public event Action OnPlayerHit;
    public event Action OnPlayerDeath;

    public void TakeDamage()
    {
        if (isInvincible) return;
        
        health--;
        scoreManager.DecreaseMultiplier();
        
        OnPlayerHit?.Invoke();
        
        if (health <= 0 ) 
        {
            Death();
            return;
        }
        
        StartCoroutine(InvincibilityCoroutine());
    }
    
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
    }

    public void Death()
    {
        OnPlayerDeath?.Invoke();
        gameObject.SetActive(false);
    }
}
