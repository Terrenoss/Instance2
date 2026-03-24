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

        Transform procBlocksTf = levelExporter.ProceduralBlocksParent;
        
        if (procBlocksTf == null)
        {
            GameObject procBlocksGO = new GameObject("ProceduralBlocks");
            procBlocksGO.transform.SetParent(levelExporter.BlocksParent);
            Undo.RegisterCreatedObjectUndo(procBlocksGO, "Create ProceduralBlocks");
            procBlocksTf = procBlocksGO.transform;
            levelExporter.ProceduralBlocksParent = procBlocksTf;
            EditorUtility.SetDirty(levelExporter);
        }
        else
        {
            for (int j = procBlocksTf.childCount - 1; j >= 0; j--)
                Undo.DestroyObjectImmediate(procBlocksTf.GetChild(j).gameObject);
        }

        List<float> waveTimes = GetWaveTimes(levelExporter, cachedLevelObjects);

        for (int i = 0; i < zones.Count; i++)
        {
            GenerateZone(levelExporter, cubePrefab, zones[i], procBlocksTf, waveTimes, i);
        }
    }

    public static void GenerateSingleZone(LevelExporter levelExporter, GameObject cubePrefab, FrequencyZone zone, int zoneIndex, LevelObject[] cachedLevelObjects)
    {
        if (levelExporter == null || levelExporter.BlocksParent == null || cubePrefab == null)
        {
            Debug.LogWarning("Missing references for procedural generation.");
            return;
        }

        Transform procBlocksTf = levelExporter.ProceduralBlocksParent;
        
        if (procBlocksTf == null)
        {
            GameObject procBlocksGO = new GameObject("ProceduralBlocks");
            procBlocksGO.transform.SetParent(levelExporter.BlocksParent);
            Undo.RegisterCreatedObjectUndo(procBlocksGO, "Create ProceduralBlocks");
            procBlocksTf = procBlocksGO.transform;
            levelExporter.ProceduralBlocksParent = procBlocksTf;
            EditorUtility.SetDirty(levelExporter);
        }

        List<float> waveTimes = GetWaveTimes(levelExporter, cachedLevelObjects);
        GenerateZone(levelExporter, cubePrefab, zone, procBlocksTf, waveTimes, zoneIndex);
    }

    private static List<float> GetWaveTimes(LevelExporter levelExporter, LevelObject[] cachedLevelObjects)
    {
        List<float> waveTimes = new List<float>();
        foreach (LevelObject obj in cachedLevelObjects)
        {
            if (obj != null && obj.type == ObstacleType.wave)
                waveTimes.Add(obj.transform.position.z / levelExporter.Speed);
        }
        return waveTimes;
    }

    private static void GenerateZone(LevelExporter levelExporter, GameObject cubePrefab, FrequencyZone zone, Transform parent, List<float> waveTimes, int zoneIndex)
    {
        string zoneContainerName = "Zone_" + zoneIndex;
        Transform existingZoneTf = parent.Find(zoneContainerName);
        if (existingZoneTf != null) Undo.DestroyObjectImmediate(existingZoneTf.gameObject);

        if (zone.probability <= 0f) return;

        GameObject zoneGO = new GameObject(zoneContainerName);
        zoneGO.transform.SetParent(parent);
        Undo.RegisterCreatedObjectUndo(zoneGO, "Create " + zoneContainerName);
        Transform zoneTf = zoneGO.transform;

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
                cube.transform.SetParent(zoneTf);
                Undo.RegisterCreatedObjectUndo(cube, "Spawn Procedural Cube");
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
