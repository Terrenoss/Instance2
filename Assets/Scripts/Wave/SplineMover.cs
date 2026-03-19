using UnityEngine;
using UnityEngine.Splines;

public class SplineMover : MonoBehaviour
{
    public SplineContainer splineToFollow;
    public float speed = 5f;

    [SerializeField][Range(0, 1)] private float waveEndPosition;
    [SerializeField] private bool isWave = false;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PoolingSystem poolingSystem;

    private float currentSplineProgression;
    private float splineLength;
    private bool isDisable = true;

    private float offset;
    private Renderer rend;

    void Start()
    {
        splineLength = splineToFollow.CalculateLength();

        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            offset = rend.bounds.extents.y;
        }
    }

    void Update()
    {
        if (isDisable) return;

        currentSplineProgression -= (speed / splineLength) * Time.deltaTime;

        Vector3 splinePos = splineToFollow.EvaluatePosition(currentSplineProgression);
        Vector3 up = splineToFollow.EvaluateUpVector(currentSplineProgression);
        Vector3 forward = splineToFollow.EvaluateTangent(currentSplineProgression);

        transform.position = splinePos + up * offset;

        transform.rotation = Quaternion.LookRotation(forward, up);

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
        if (!isActive)
        {
            if (poolingSystem != null)
                poolingSystem.Release(gameObject);
        }

        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider) collider.enabled = isActive;

        if (rend != null)
            rend.enabled = isActive;

        isDisable = !isActive;
    }
}