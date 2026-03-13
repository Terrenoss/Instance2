using UnityEngine;

public class HitPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.GetComponent<PlayerHealth>() != null)
        {
            EventManager.Instance.PlayerHitFunc();
        }
    }
}
