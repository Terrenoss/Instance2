using UnityEngine;
using UnityEngine.Splines;

public class SplineMover : MonoBehaviour
{
    public SplineContainer splineToFollow;
    [SerializeField] private float speed = 5f;
    [SerializeField] [Range(0,1)] private float waveEndPosition;
    [SerializeField] private bool isWave = false;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PoolingSystem poolingSystem;

    private float currentSplineProgression;
    private float splineLength;
    private bool isDisable = true;

    void Start()
    {
        splineLength = splineToFollow.CalculateLength();
    }

    void Update()
    {
        if (isDisable) return;

        currentSplineProgression -= (speed / splineLength) * Time.deltaTime;

        transform.position = splineToFollow.EvaluatePosition(currentSplineProgression);
        transform.rotation = Quaternion.LookRotation(transform.forward, splineToFollow.EvaluateUpVector(currentSplineProgression));

        if (currentSplineProgression <= waveEndPosition)
        {
            UpdateWave(false);

            if (isWave && playerHealth != null)
                playerHealth.TakeDamage();
        }
    }

    public void ResetWave()
    {
        currentSplineProgression = 1f;
        UpdateWave(true);
        isDisable = false;
    }

    public void UpdateWave(bool isActive)
    {
        if (isActive == false)
        {
            if (poolingSystem != null)
                poolingSystem.Release(gameObject);
        }
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        if (collider) collider.enabled = isActive;

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer) renderer.enabled = isActive;

        isDisable = !isActive;
    }
}