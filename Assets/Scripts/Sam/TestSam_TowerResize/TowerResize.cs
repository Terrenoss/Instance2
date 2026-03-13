using UnityEngine;

public class TowerResize : MonoBehaviour
{
    [SerializeField] private float expansionSpeed = 0.2f;
    [SerializeField] private float downMovementSpeed = 0.8f;

    private void Update()
    {
        transform.localScale = new Vector3(transform.localScale.x + expansionSpeed * Time.deltaTime,
                                           transform.localScale.y,
                                           transform.localScale.z + expansionSpeed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x,
                                         transform.position.y - downMovementSpeed * Time.deltaTime,
                                         transform.position.z);
    }
}
