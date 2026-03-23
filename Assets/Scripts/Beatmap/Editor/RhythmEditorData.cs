using UnityEngine;

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
