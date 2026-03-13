using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 3;
    [SerializeField] private ScoreManager scoreManager;

    private void OnTriggerEnter(Collider coll)
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
