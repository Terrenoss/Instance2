using UnityEngine;
using UnityEngine.Splines;

public class TimeOffset : MonoBehaviour
{
    [SerializeField] private SplineMover splineMover;

    [Range(0f, 1f)]
    [SerializeField] private float startOffset = 0f;

    [Range(0f, 1f)]
    [SerializeField] private float endOffset = 1f;

    public float totalTime;

    void Awake()
    {
        if (splineMover == null)
        {
            Debug.LogError("SplineMover non assigné !");
            return;
        }

        if (endOffset <= startOffset)
        {
            Debug.LogError("endOffset doit être supérieur à startOffset !");
            return;
        }

        float splineLength = splineMover.splineToFollow.CalculateLength();

        float percentage = endOffset - startOffset;
        float distanceToTravel = splineLength * percentage;

        totalTime = distanceToTravel / splineMover.speed;

        Debug.Log($"Distance parcourue : {distanceToTravel}");
        Debug.Log($"Temps pour parcourir {percentage * 100}% de la spline : {totalTime} secondes.");
    }
    
    void OnDrawGizmos()
    {
        if (splineMover == null || splineMover.splineToFollow == null)
            return;

        Gizmos.color = Color.green;
        Vector3 startPos = splineMover.splineToFollow.EvaluatePosition(startOffset);
        Gizmos.DrawSphere(startPos, 0.2f);

        Gizmos.color = Color.red;
        Vector3 endPos = splineMover.splineToFollow.EvaluatePosition(endOffset);
        Gizmos.DrawSphere(endPos, 0.2f);
    }
    
}

