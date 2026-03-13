using UnityEngine;

public class CheckVictory : MonoBehaviour
{
    private void OnTriggerEnter (Collider coll)
    {
        if (coll.gameObject.GetComponent<PlayerHealth>() != null)
        {
            EventManager.Instance.VictoryFunc();
        }
    }
}
