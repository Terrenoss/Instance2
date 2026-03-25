using System;
using UnityEngine;

public class TowerMove : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private Spawner spawner;
    private float towerProgress;

    private void Start()
    {
        transform.position = startPoint.position;
    }

    void Update()
    {
        if (spawner == null || spawner.LevelDuration <= 0f)
        {
            return;
        }

        towerProgress = Mathf.Clamp01(spawner.CurrentTime / spawner.LevelDuration);

        transform.position = Vector3.Lerp(
            startPoint.position,
            endPoint.position,
            towerProgress
        );
    }
}