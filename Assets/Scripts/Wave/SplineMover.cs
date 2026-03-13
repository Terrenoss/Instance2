using UnityEngine;
using UnityEngine.Splines;

public class SplineMover : MonoBehaviour
{
    public SplineContainer splineToFollow;
    [SerializeField] private float speed = 5f;
    [SerializeField] [Range(0,1)] private float waveEndPosition;
    [SerializeField] private bool isWave = false;

    private float currentSplineProgression;
    private float splineLength;
    private Vector3 wavePosition = Vector3.zero;
    private Vector3 splineUpVector = Vector3.zero;
    private bool isDisable = false;

    void Start()
    {
        splineLength = splineToFollow.CalculateLength();
    }

    void Update()
    {
        if (currentSplineProgression > waveEndPosition && !isDisable)
        {
            UpdateWave(false);
            if (isWave)
            {
                //pulling system
                //perd une vie
            }
            isDisable = true;
        }

        if (isDisable) return;
        
        currentSplineProgression += (speed / splineLength) * Time.deltaTime;

        wavePosition = splineToFollow.EvaluatePosition(currentSplineProgression);
        transform.position = wavePosition;
        
        splineUpVector = splineToFollow.EvaluateUpVector(currentSplineProgression);
        
        transform.rotation = Quaternion.LookRotation(transform.forward, splineUpVector);

    }

    public void UpdateWave(bool isActive)
    {
        gameObject.GetComponent<BoxCollider>().enabled = isActive;
        gameObject.GetComponent<MeshRenderer>().enabled = isActive;
        isDisable = !isActive;
    }
}