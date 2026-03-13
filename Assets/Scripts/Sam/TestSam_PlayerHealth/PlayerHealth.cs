using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 3;
    [SerializeField] private ScoreManager scoreManager;

    private void Start()
    {
        EventManager.Instance.PlayerHit += Hit;
    }

    private void Hit()
    {
        TakeDamage();
        EventManager.Instance.PlayerHitFunc();

        if (health <= 0 ) {
            EventManager.Instance.PlayerDeathFunc();
        }
    }

    public void TakeDamage()
    {
        health--;
        scoreManager.DecreaseMultiplier();
    }
}
