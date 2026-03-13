using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public int health = 3;

    private void Start()
    {
        EventManager.Instance.PlayerHit += Hit;
    }

    private void Hit()
    {
        health--;

        if (health <= 0 ) {
            EventManager.Instance.PlayerDeathFunc();
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.PlayerHit -= Hit;
    }
}
