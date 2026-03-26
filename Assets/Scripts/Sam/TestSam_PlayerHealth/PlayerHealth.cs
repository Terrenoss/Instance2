using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 3;
    
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private AudioManager audioManager;
    
    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    private bool isInvincible = false;
    [SerializeField] private InvincibilityBlink invincibilityEffect;
    [SerializeField] private ShakeCamera shakeCamera;
    [SerializeField] private VFXPlayerDamage vfxPlayerDamage;

    public event Action OnPlayerHit;
    public event Action OnPlayerDeath;

    public void TakeDamage()
    {
        if (isInvincible) return;
        
        health--;
        
        OnPlayerHit?.Invoke();
        if (audioManager != null) audioManager.PlaySound("Degat");
        
        shakeCamera.AddTrauma(0.6f);
        
        scoreManager.DecreaseMultiplier();
        
        vfxPlayerDamage.PlayVfx(health);
        
        if (health <= 0 ) 
        {
            shakeCamera.AddTrauma(150f);
            Death();
            return;
        }
        
        StartCoroutine(InvincibilityCoroutine());
    }
    
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        invincibilityEffect.StartInvincibility();
        
        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
    }

    public void Death()
    {
        if (audioManager != null) audioManager.PlaySound("Mort");
        OnPlayerDeath?.Invoke();
        scoreManager.CheckBestScore();
        gameObject.SetActive(false);
    }
}
