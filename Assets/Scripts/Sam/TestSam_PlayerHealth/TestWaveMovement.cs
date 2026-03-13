using UnityEngine;

public class TestWaveMovement : MonoBehaviour
{
    [SerializeField] private int waveSpeed = 7;

    private void Update()
    {
        transform.position = new Vector3(transform.position.x,
                                         transform.position.y,
                                         transform.position.z - waveSpeed * Time.deltaTime);
    }
}
