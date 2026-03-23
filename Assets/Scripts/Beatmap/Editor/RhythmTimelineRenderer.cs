#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class RhythmTimelineRenderer
{
    private const float TIMELINE_HEIGHT = 100f;
    private const float WAVEFORM_HEIGHT_MULTIPLIER = 50f;

    public static void DrawBackground(Vector2 scrollPosition, float positionWidth)
    {
        EditorGUI.DrawRect(new Rect(scrollPosition.x, 0, positionWidth, TIMELINE_HEIGHT), new Color(0.15f, 0.15f, 0.15f, 1f));
    }

    public static void DrawWaveform(IRhythmEditorContext context, float totalWidth, Vector2 scrollPosition, float positionWidth, float waveformContrast)
    {
        var audioController = context.AudioController;
        if (Event.current.type != EventType.Repaint || audioController.CachedSamples == null || audioController.Clip == null) return;

        Color waveColor = new Color(1f, 0.6f, 0f, 1f);
        float startX = scrollPosition.x;
        float endX = startX + positionWidth;
        float unitsPerPixel = audioController.Clip.length / totalWidth; 
        int sampleRate = audioController.Clip.frequency * audioController.Clip.channels;
        float[] samples = audioController.CachedSamples;

        for (float x = startX; x <= endX; x++)
        {
            float timeStart = x * unitsPerPixel;
            float timeEnd = (x + 1) * unitsPerPixel;

            int sStart = Mathf.Clamp(Mathf.FloorToInt(timeStart * sampleRate), 0, samples.Length);
            int sEnd = Mathf.Clamp(Mathf.FloorToInt(timeEnd * sampleRate), 0, samples.Length);
            int count = sEnd - sStart;

            float max = 0;
            if (count > 0) 
            {
                for (int j = sStart; j < sEnd; j++)
                {
                    float val = Mathf.Abs(samples[j]);
                    if (val > max) max = val;
                }
            }
            
            float displayMax = Mathf.Pow(max, waveformContrast);
            float h = displayMax * WAVEFORM_HEIGHT_MULTIPLIER;
            EditorGUI.DrawRect(new Rect(x, WAVEFORM_HEIGHT_MULTIPLIER - h, 1, h * 2), waveColor);
        }
    }

    public static void DrawZones(IRhythmEditorContext context, float totalWidth, bool isHoveringWave)
    {
        var audioClip = context.AudioController.Clip;
        if (audioClip == null) return;

        foreach (var zone in context.Zones)
        {
            float startXZone = totalWidth * (zone.startTime / audioClip.length);
            float endXZone = totalWidth * (zone.endTime / audioClip.length);
            
            EditorGUI.DrawRect(new Rect(startXZone, 0, endXZone - startXZone, TIMELINE_HEIGHT), zone.zoneColor);
            
            Color solidColor = context.SelectedZones.Contains(zone) 
                ? new Color(1f - zone.zoneColor.r, 1f - zone.zoneColor.g, 1f - zone.zoneColor.b, 1f) 
                : new Color(zone.zoneColor.r, zone.zoneColor.g, zone.zoneColor.b, 1f);

            EditorGUI.DrawRect(new Rect(startXZone, 0, endXZone - startXZone, 4), solidColor);
            EditorGUI.DrawRect(new Rect(startXZone, TIMELINE_HEIGHT - 4, endXZone - startXZone, 4), solidColor);

            if (!isHoveringWave)
            {
                EditorGUIUtility.AddCursorRect(new Rect(startXZone - 5, 0, 10, TIMELINE_HEIGHT), MouseCursor.ResizeHorizontal);
                EditorGUIUtility.AddCursorRect(new Rect(endXZone - 5, 0, 10, TIMELINE_HEIGHT), MouseCursor.ResizeHorizontal);
                EditorGUIUtility.AddCursorRect(new Rect(startXZone + 5, 0, (endXZone - startXZone) - 10, TIMELINE_HEIGHT), MouseCursor.Pan);
            }
        }
    }

    public static void DrawPlayhead(IRhythmEditorContext context, float totalWidth)
    {
        if (context.AudioController.Clip == null) return;
        float progress = context.AudioController.CurrentTime / context.AudioController.Clip.length;
        float playheadX = totalWidth * progress;
        
        EditorGUI.DrawRect(new Rect(playheadX, 0, 2, TIMELINE_HEIGHT), Color.red);
        EditorGUI.DrawRect(new Rect(playheadX - 4, 0, 10, 10), Color.red);
    }

    public static void DrawWaves(IRhythmEditorContext context, float totalWidth)
    {
        var audioClip = context.AudioController.Clip;
        var levelExporter = context.LevelExporter;
        
        if (levelExporter == null || audioClip == null || Event.current.type != EventType.Repaint) return;

        foreach (LevelObject obj in context.CachedLevelObjects)
        {
            if (obj == null || obj.type != ObstacleType.wave) continue;
            
            float timeOfWave = obj.transform.position.z / levelExporter.Speed;
            float waveProgress = timeOfWave / audioClip.length;
            float waveX = totalWidth * waveProgress;
            
            if (waveX >= 0 && waveX <= totalWidth)
            {
                if (context.SelectedWaves.Contains(obj))
                    EditorGUI.DrawRect(new Rect(waveX - 1, 0, 4, TIMELINE_HEIGHT), Color.green);
                else
                    EditorGUI.DrawRect(new Rect(waveX, 0, 2, TIMELINE_HEIGHT), Color.cyan);

                EditorGUIUtility.AddCursorRect(new Rect(waveX - 3, 0, 6, TIMELINE_HEIGHT), MouseCursor.Link);
            }
        }
    }
}
#endif
