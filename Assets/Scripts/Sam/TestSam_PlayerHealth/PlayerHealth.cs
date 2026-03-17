using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public int health = 3;

    private void OnTriggerEnter(Collider coll)
    {
        health--;
        EventManager.Instance.PlayerHitFunc();

        if (health <= 0 ) {
            EventManager.Instance.PlayerDeathFunc();
        }
    }
}
