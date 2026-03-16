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

    [Header("Prefabs")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject wavePrefab;
    
    private float levelTime = 0f;
    [SerializeField] private PoolingSystem poolingSystem;
    
    private List<ExportData> notes = new List<ExportData>();
    private int nextNoteIndex = 0;
    public event Action OnVictory;


    void Start()
    {
        LoadLevel();
    }

    void Update()
    {
        levelTime += Time.deltaTime;

        while (nextNoteIndex < notes.Count && levelTime >= notes[nextNoteIndex].time)
        {
            Spawn(notes[nextNoteIndex]);
            nextNoteIndex++;
        }
        
        if (nextNoteIndex >= notes.Count && poolingSystem.AreAllBlocksInactive())
        {
            OnVictory?.Invoke();
        }
    }

    void LoadLevel()
    {
        string path = Application.dataPath + "/Levels/level.json";        
        if (!File.Exists(path))
        {
            Debug.LogError("Level JSON not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        ExportWrapper wrapper = JsonUtility.FromJson<ExportWrapper>(json);
        notes = wrapper.objects;

        notes.Sort((a, b) => a.time.CompareTo(b.time));
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
            wave.SetWaveType(data.waveType);
        }
    }
}