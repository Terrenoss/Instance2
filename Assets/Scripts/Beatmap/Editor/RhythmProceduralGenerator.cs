#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class RhythmProceduralGenerator
{
    public static void GenerateProceduralBlocks(LevelExporter levelExporter, GameObject cubePrefab, AudioClip audioClip, List<FrequencyZone> zones, LevelObject[] cachedLevelObjects)
    {
        if (levelExporter == null || levelExporter.BlocksParent == null || cubePrefab == null)
        {
            Debug.LogWarning("Missing references for procedural generation.");
            return;
        }

        Transform blocksParent = levelExporter.BlocksParent;
        Transform procBlocksTf = blocksParent.Find("ProceduralBlocks");
        if (procBlocksTf != null) Undo.DestroyObjectImmediate(procBlocksTf.gameObject);

        GameObject procBlocksGO = new GameObject("ProceduralBlocks");
        procBlocksGO.transform.SetParent(blocksParent);
        Undo.RegisterCreatedObjectUndo(procBlocksGO, "Create ProceduralBlocks");
        procBlocksTf = procBlocksGO.transform;

        List<float> waveTimes = new List<float>();
        foreach (LevelObject obj in cachedLevelObjects)
        {
            if (obj != null && obj.type == ObstacleType.wave)
            {
                waveTimes.Add(obj.transform.position.z / levelExporter.Speed);
            }
        }

        foreach (FrequencyZone zone in zones)
        {
            if (zone.probability <= 0f) continue;

            for (float t = zone.startTime; t <= zone.endTime; t += zone.beatInterval)
            {
                if (IsTimeTooCloseToWaves(t, waveTimes, zone.safetyMargin)) continue;

                int safeLane = Random.Range(0, 3);
                for (int lane = 0; lane < 3; lane++)
                {
                    if (lane == safeLane) continue;
                    if (Random.value > zone.probability) continue;

                    GameObject cube = (GameObject)PrefabUtility.InstantiatePrefab(cubePrefab);
                    
                    float xPos = levelExporter.Player.position.x;
                    if (lane == 0) xPos -= zone.laneOffset;
                    else if (lane == 2) xPos += zone.laneOffset;

                    float zPos = t * levelExporter.Speed;
                    cube.transform.position = new Vector3(xPos, cube.transform.position.y, zPos);
                    cube.transform.SetParent(procBlocksTf);
                    Undo.RegisterCreatedObjectUndo(cube, "Spawn Procedural Cube");
                }
            }
        }
    }

    private static bool IsTimeTooCloseToWaves(float time, List<float> waveTimes, float safetyMargin)
    {
        foreach (float waveTime in waveTimes)
        {
            if (Mathf.Abs(time - waveTime) < safetyMargin) return true;
        }
        return false;
    }
}
#endif
