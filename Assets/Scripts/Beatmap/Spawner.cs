using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Splines;

public class Spawner : MonoBehaviour
{
    [Header("Splines")]
    [SerializeField] private SplineContainer leftSpline;
    [SerializeField] private SplineContainer centerSpline;
    [SerializeField] private SplineContainer rightSpline;
    [SerializeField] private SplineMover splineMover;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Animator animator;
    
    private float levelTime = 0f;
    private float musicTimeStart = 0f;
    [SerializeField] private PoolingSystem poolingSystem;
    
    private List<ExportData> notes = new List<ExportData>();
    private int nextNoteIndex = 0;
    public event Action OnVictory;
    
    private bool isLevelStarted = false;
    
    public float LevelDuration { get; private set; }
    public float CurrentTime => levelTime;

    [SerializeField] private TimeOffset timeOffset;

    void Start()
    {
        LoadLevel();
        musicTimeStart = timeOffset.totalTime;
    }

    void Update()
    {
        levelTime += Time.deltaTime;

        if (levelTime >= musicTimeStart && isLevelStarted == false)
        {
            isLevelStarted = true;
            audioManager.PlaySound("MainMusic");
        }
        
        while (nextNoteIndex < notes.Count && levelTime >= notes[nextNoteIndex].time)
        {
            Spawn(notes[nextNoteIndex]);
            nextNoteIndex++;
        }

        if (nextNoteIndex >= notes.Count && poolingSystem.AreAllBlocksInactive())
        {
            OnVictory?.Invoke();
            Debug.Log("All blocks are inactive");
        }
    }

    void LoadLevel()
    {
        string persistentPath = Path.Combine(Application.persistentDataPath, "Levels/level.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "Levels/level.json");

        if (!File.Exists(persistentPath))
        {
            if (File.Exists(streamingPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(persistentPath));
                File.Copy(streamingPath, persistentPath);
                Debug.Log("Level copied from StreamingAssets to persistentDataPath");
            }
            else
            {
                Debug.LogError("No level found in StreamingAssets or persistentDataPath");
                return;
            }
        }

        string json = File.ReadAllText(persistentPath);
        ExportWrapper wrapper = JsonUtility.FromJson<ExportWrapper>(json);
        notes = wrapper.objects;

        notes.Sort((a, b) => a.time.CompareTo(b.time));

        if (notes.Count > 0)
        {
            float splineLength = GetSplineLength(centerSpline);

            float speed = splineMover.speed;

            float travelTime = splineLength / speed;

            LevelDuration = notes[notes.Count - 1].time + travelTime;
        }
    }
    
    void Spawn(ExportData data)
    {
        GameObject obj = poolingSystem.GetAvailable(data.type);

        if (obj == null)
        {
            Debug.LogWarning("No available object in pool for type " + data.type);
            return;
        }

        SplineContainer splineToUse = null;

        switch (data.lane)
        {
            case Lane.Left: 
                splineToUse = leftSpline; 
                break;
            case Lane.Center: 
                splineToUse = centerSpline; 
                break;
            case Lane.Right: 
                splineToUse = rightSpline; 
                break;
        }

        SplineMover mover = obj.GetComponent<SplineMover>();
        if (mover != null)
        {
            mover.splineToFollow = splineToUse;
            mover.ResetWave();
        }
        WaveType wave = obj.GetComponent<WaveType>();
        if (wave != null)
        {
            animator.SetTrigger("Spawn");
            wave.SetWaveType(data.waveType);
            wave.SetParryKeyVisible(data.isParryKeyVisible);
        }
    }
    
    float GetSplineLength(SplineContainer spline, int resolution = 50)
    {
        float length = 0f;
        Vector3 previousPoint = spline.EvaluatePosition(0f);

        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 point = spline.EvaluatePosition(t);
            length += Vector3.Distance(previousPoint, point);
            previousPoint = point;
        }

        return length;
    }
}