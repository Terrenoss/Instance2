using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveParry : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private float perfectDistance = 0.4f;
    [SerializeField] private Vector3 boxSize = new(2f, 2f, 2f);

    public void Parry(Guid id)
    {
        Collider[] hits = Physics.OverlapBox(transform.position, boxSize / 2, Quaternion.identity);
        
        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out WaveType waveType)) continue;
            
            float distance = Vector3.Distance(transform.position, hit.transform.position);

            if (id == waveType.waveParam[waveType.waveTypeEnum].keyId)
            {
                if (distance <= perfectDistance)
                {
                    scoreManager.AddParryScore();
                    scoreManager.IncreaseMultiplier();
                }
                else
                {
                    scoreManager.AddParryScore();
                }
            }
            else
            {
                //perd une vie
            }
            hit.TryGetComponent(out SplineMover waveMover);
            waveMover.UpdateWave(false);
            //pulling system
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, boxSize);
        
        DrawCube(perfectDistance * 2, Color.green);
    }

    void DrawCube(float size, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(transform.position, new Vector3(size, size, size));
    }
}