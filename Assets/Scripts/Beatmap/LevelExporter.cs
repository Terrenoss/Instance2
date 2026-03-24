using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class FrequencyZone
{
    public float startTime = 0f;
    public float endTime = 10f;
    [Range(0f, 1f)] public float probability = 0.5f;
    
    public float beatInterval = 0.5f;
    public float laneOffset = 2f;
    public float safetyMargin = 1.5f;
    
    public Color zoneColor = new Color(0f, 1f, 0f, 0.3f);
}

public class LevelExporter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform blocksParent;
    [SerializeField] private Transform proceduralBlocksParent;
    [SerializeField] private float speed = 10f;
    public List<FrequencyZone> zones = new List<FrequencyZone>();

    public float Speed => speed;
    public Transform BlocksParent => blocksParent;
    public Transform ProceduralBlocksParent 
    {
        get => proceduralBlocksParent;
        set => proceduralBlocksParent = value;
    }
    public Transform Player => player;

    public void ExportLevel()
    {
        LevelObject[] levelObjects = blocksParent.GetComponentsInChildren<LevelObject>();

        List<ExportData> data = new List<ExportData>();

        foreach (LevelObject levelObj in levelObjects)
        {
            Transform block = levelObj.transform;

            ExportData entry = new ExportData
            {
                time = GetTime(block),
                lane = GetLane(block.position.x),
                type = levelObj.type,
                waveType = levelObj.waveType,
                isParryKeyVisible = levelObj.isParryKeyVisible
            };

            data.Add(entry);
        }

        data.Sort((a, b) => a.time.CompareTo(b.time));

        ExportWrapper wrapper = new ExportWrapper { objects = data };

        string folderPath = Path.Combine(Application.persistentDataPath, "Levels");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string path = Path.Combine(folderPath, "level.json");

        File.WriteAllText(path, JsonUtility.ToJson(wrapper, true));
        Debug.Log("Level exported to: " + path);
    }

    Lane GetLane(float objectX)
    {
        float playerX = player.position.x;

        if (objectX < playerX)
            return Lane.Left;
        if (objectX > playerX)
            return Lane.Right;
        return Lane.Center;
    }

    float GetTime(Transform block)
    {
        float distanceZ = Mathf.Abs(block.position.z - player.position.z);
        return distanceZ / speed;
    }
}

[System.Serializable]
public enum Lane
{
    Left,
    Center,
    Right
}

[System.Serializable]
public class ExportData
{
    public float time;
    public Lane lane;
    public ObstacleType type;
    public WaveTypeSelection waveType;
    public bool isParryKeyVisible;
}

[System.Serializable]
public class ExportWrapper
{
    public List<ExportData> objects;
}