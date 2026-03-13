using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 3;
    
    [SerializeField] private ScoreManager scoreManager;

    public void TakeDamage()
    {
        health--;
        scoreManager.DecreaseMultiplier();
        
        if (health <= 0 ) {
            Death();
        }
    }

    public void Death()
    {
        
        gameObject.SetActive(false);
    }
}
