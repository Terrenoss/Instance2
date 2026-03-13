using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveParry : MonoBehaviour
{
    public Vector3 boxSize = new Vector3(2f, 2f, 2f);
    
    public void Parry(Guid id)
    {
        Collider[] hits = Physics.OverlapBox(transform.position, boxSize / 2, Quaternion.identity);
        
        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out WaveType waveType)) continue;

            if (id == waveType.waveParam[waveType.waveTypeEnum].keyId)
            {
                //score ++
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
    }
}