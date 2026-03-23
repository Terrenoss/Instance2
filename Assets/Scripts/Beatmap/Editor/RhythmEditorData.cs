using UnityEngine;
using System.Collections.Generic;

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

public interface IRhythmEditorContext
{
    RhythmAudioController AudioController { get; }
    LevelExporter LevelExporter { get; }
    List<LevelObject> SelectedWaves { get; }
    List<FrequencyZone> Zones { get; }
    List<FrequencyZone> SelectedZones { get; }
    LevelObject[] CachedLevelObjects { get; }
    
    GameObject WavePrefab { get; set; }
    GameObject CubePrefab { get; set; }
    bool IsRecording { get; set; }
    bool ShowHelp { get; set; }
    
    void RepaintWindow();
}
