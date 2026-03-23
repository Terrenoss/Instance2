#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RhythmTimelineInteraction
{
    private RhythmEditorWindow window;
    
    private const float CLICK_TOLERANCE = 5f;
    private const float NUDGE_AMOUNT = 0.01f;
    private const float ZOOM_SPEED = 0.2f;
    private const float MIN_ZOOM = 1f;
    private const float MAX_ZOOM = 100f;

    public bool isDraggingWave = false;
    public float dragOffsetTime = 0f;

    public FrequencyZone draggingZone = null;
    public bool isDraggingZoneStart = false;
    public bool isDraggingZoneBody = false;
    public float dragZoneOffsetTime = 0f;

    public RhythmTimelineInteraction(RhythmEditorWindow window)
    {
        this.window = window;
    }

    private float GetTimeAtMouse(float mouseX, float scrollX, float totalWidth, float clipLength)
    {
        float virtualMouseX = mouseX + scrollX;
        return (virtualMouseX / totalWidth) * clipLength;
    }

    public void HandleGlobalShortcuts(RhythmTimelineUI timelineUI)
    {
        if (Event.current.type == EventType.KeyDown && GUIUtility.keyboardControl == 0)
        {
            if (Event.current.keyCode == KeyCode.R)
            {
                timelineUI.zoomLevel = MIN_ZOOM;
                timelineUI.scrollPosition.x = 0f;
                Event.current.Use();
                window.Repaint();
            }
            else if (Event.current.keyCode == KeyCode.LeftArrow || Event.current.keyCode == KeyCode.RightArrow)
            {
                float nudge = (Event.current.keyCode == KeyCode.RightArrow) ? NUDGE_AMOUNT : -NUDGE_AMOUNT;
                if (Event.current.shift) nudge *= 5f; 

                if (window.selectedWaves.Count > 0 && window.levelExporter != null)
                {
                    float deltaZ = nudge * window.levelExporter.Speed;
                    foreach (var wave in window.selectedWaves)
                    {
                        Undo.RecordObject(wave.transform, "Nudge Wave");
                        wave.transform.position += new Vector3(0, 0, deltaZ);
                    }
                    Event.current.Use();
                    window.Repaint();
                }
                else if (window.selectedZones.Count > 0)
                {
                    foreach (var zone in window.selectedZones)
                    {
                        float duration = zone.endTime - zone.startTime;
                        float maxStart = (window.audioController.Clip != null) ? window.audioController.Clip.length - duration : 999f;
                        
                        zone.startTime = Mathf.Clamp(zone.startTime + nudge, 0f, maxStart);
                        zone.endTime = zone.startTime + duration;
                    }
                    Event.current.Use();
                    window.Repaint();
                }
            }
        }
    }

    public void HandleScrollAndZoom(Rect scrollViewRect, float totalWidth, float positionWidth, RhythmTimelineUI timelineUI)
    {
        Event e = Event.current;
        if (e.type == EventType.ScrollWheel && scrollViewRect.Contains(e.mousePosition))
        {
            if (e.control || e.command) 
            {
                float timeAtMouse = (timelineUI.scrollPosition.x + e.mousePosition.x) / totalWidth;
                timelineUI.zoomLevel -= e.delta.y * ZOOM_SPEED; 
                timelineUI.zoomLevel = Mathf.Clamp(timelineUI.zoomLevel, MIN_ZOOM, MAX_ZOOM);

                float newTotalWidth = positionWidth * timelineUI.zoomLevel;
                if (newTotalWidth < positionWidth) newTotalWidth = positionWidth;

                timelineUI.scrollPosition.x = (timeAtMouse * newTotalWidth) - e.mousePosition.x;
            }
            else
            {
                timelineUI.scrollPosition.x += e.delta.y * 50f; 
                timelineUI.scrollPosition.x += e.delta.x * 50f;
                window.audioController.autoScroll = false;
            }

            e.Use(); 
            window.Repaint();
        }
    }

    public void HandleWaveDrag(float totalWidth, float scrollPositionX)
    {
        Event e = Event.current;
        var audioClip = window.audioController.Clip;
        
        if (e.type == EventType.MouseUp)
        {
            isDraggingWave = false;
            draggingZone = null;
            isDraggingZoneBody = false;
        }

        if (isDraggingWave && window.selectedWaves.Count > 0 && e.type == EventType.MouseDrag)
        {
            float timeAtMouse = GetTimeAtMouse(e.mousePosition.x, scrollPositionX, totalWidth, audioClip.length);
            float newTime = Mathf.Clamp(timeAtMouse - dragOffsetTime, 0f, audioClip.length);
            float newZPos = newTime * window.levelExporter.Speed;
            
            LevelObject active = Selection.activeGameObject?.GetComponent<LevelObject>();
            if (active != null)
            {
                float deltaZ = newZPos - active.transform.position.z;
                foreach (var wave in window.selectedWaves)
                {
                    Undo.RecordObject(wave.transform, "Move Wave");
                    wave.transform.position = new Vector3(wave.transform.position.x, wave.transform.position.y, wave.transform.position.z + deltaZ);
                }
            }
            
            e.Use();
            window.Repaint();
        }
    }

    public void HandleZoneDrag(float totalWidth, float scrollPositionX)
    {
        Event e = Event.current;
        if (draggingZone != null && e.type == EventType.MouseDrag)
        {
            var audioClip = window.audioController.Clip;
            float timeAtMouse = GetTimeAtMouse(e.mousePosition.x, scrollPositionX, totalWidth, audioClip.length);

            if (isDraggingZoneBody)
            {
                float newStartTime = timeAtMouse - dragZoneOffsetTime;
                float delta = newStartTime - draggingZone.startTime;

                if (window.selectedZones.Contains(draggingZone) && window.selectedZones.Count > 1)
                {
                    float minStart = 0f;
                    foreach (var z in window.selectedZones) if (z.startTime + delta < minStart) minStart = z.startTime + delta;
                    if (minStart < 0f) delta -= minStart; 

                    foreach (var z in window.selectedZones)
                    {
                        float duration = z.endTime - z.startTime;
                        z.startTime += delta;
                        z.endTime = z.startTime + duration;
                    }
                }
                else
                {
                    float duration = draggingZone.endTime - draggingZone.startTime;
                    draggingZone.startTime = Mathf.Max(0, newStartTime);
                    draggingZone.endTime = draggingZone.startTime + duration;
                }
            }
            else if (isDraggingZoneStart)
            {
                draggingZone.startTime = Mathf.Clamp(timeAtMouse, 0f, draggingZone.endTime - 0.01f);
            }
            else
            {
                draggingZone.endTime = Mathf.Clamp(timeAtMouse, draggingZone.startTime + 0.01f, audioClip.length);
            }
            e.Use();
            window.Repaint();
        }
    }

    public void HandleClicks(Rect contentRect, float totalWidth, float scrollPositionX)
    {
        Event e = Event.current;
        if (!contentRect.Contains(e.mousePosition) || isDraggingWave || draggingZone != null) return;
        if (e.type != EventType.MouseDown && e.type != EventType.MouseDrag) return;

        if (TryHandleWaveClick(totalWidth)) return;
        if (TryHandleZoneClick(totalWidth, scrollPositionX)) return;
        
        HandleTimelineBackgroundClick(totalWidth);
    }

    private bool TryHandleWaveClick(float totalWidth)
    {
        if (window.levelExporter == null) return false;
        if (Event.current.type != EventType.MouseDown) return false;

        Event e = Event.current;
        var audioClip = window.audioController.Clip;

        foreach (LevelObject obj in window.cachedLevelObjects)
        {
            if (obj == null || obj.type != ObstacleType.wave) continue;
            
            float timeOfWave = obj.transform.position.z / window.levelExporter.Speed;
            float waveX = totalWidth * (timeOfWave / audioClip.length);
            
            if (waveX >= 0 && waveX <= totalWidth && Mathf.Abs(e.mousePosition.x - waveX) <= CLICK_TOLERANCE)
            {
                window.selectedZones.Clear(); 
                
                if (e.shift || EditorGUI.actionKey)
                {
                    if (window.selectedWaves.Contains(obj)) window.selectedWaves.Remove(obj);
                    else window.selectedWaves.Add(obj);
                }
                else if (!window.selectedWaves.Contains(obj))
                {
                    window.selectedWaves.Clear();
                    window.selectedWaves.Add(obj);
                }

                Selection.activeGameObject = obj.gameObject;
                isDraggingWave = true;
                
                float clickTime = (e.mousePosition.x / totalWidth) * audioClip.length;
                dragOffsetTime = clickTime - timeOfWave;
                
                if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.FrameSelected();
                
                e.Use(); 
                return true;
            }
        }
        return false;
    }

    private bool TryHandleZoneClick(float totalWidth, float scrollPositionX)
    {
        if (Event.current.type != EventType.MouseDown) return false;

        Event e = Event.current;
        var audioClip = window.audioController.Clip;

        for (int i = window.zones.Count - 1; i >= 0; i--)
        {
            var zone = window.zones[i];
            float startXZone = totalWidth * (zone.startTime / audioClip.length);
            float endXZone = totalWidth * (zone.endTime / audioClip.length);

            if (Mathf.Abs(e.mousePosition.x - startXZone) <= CLICK_TOLERANCE)
            {
                draggingZone = zone; isDraggingZoneStart = true;
                HandleZoneSelection(zone, e); e.Use(); return true;
            }
            else if (Mathf.Abs(e.mousePosition.x - endXZone) <= CLICK_TOLERANCE)
            {
                draggingZone = zone; isDraggingZoneStart = false;
                HandleZoneSelection(zone, e); e.Use(); return true;
            }
            else if (e.mousePosition.x > startXZone && e.mousePosition.x < endXZone)
            {
                draggingZone = zone; isDraggingZoneBody = true;
                HandleZoneSelection(zone, e);
                
                dragZoneOffsetTime = GetTimeAtMouse(e.mousePosition.x, scrollPositionX, totalWidth, audioClip.length) - zone.startTime;
                e.Use(); return true;
            }
        }
        return false;
    }

    private void HandleTimelineBackgroundClick(float totalWidth)
    {
        Event e = Event.current;
        var audioClip = window.audioController.Clip;

        float clickProgress = e.mousePosition.x / totalWidth;
        window.audioController.SetAudioTime(Mathf.Clamp(clickProgress * audioClip.length, 0f, audioClip.length));
        window.Repaint();
        
        if (e.type == EventType.MouseDown)
        {
            window.selectedWaves.Clear();
            Selection.activeGameObject = null;
            window.selectedZones.Clear();

            if (e.clickCount == 2) window.audioController.PlayAudio();
        }
        e.Use();
    }

    private void HandleZoneSelection(FrequencyZone zone, Event e)
    {
        window.selectedWaves.Clear(); 
        Selection.activeGameObject = null;

        if (e.shift || EditorGUI.actionKey)
        {
            if (window.selectedZones.Contains(zone)) window.selectedZones.Remove(zone);
            else window.selectedZones.Add(zone);
        }
        else if (!window.selectedZones.Contains(zone))
        {
            window.selectedZones.Clear();
            window.selectedZones.Add(zone);
        }
    }

    public bool IsHoveringWave(float totalWidth, Event e)
    {
        if (window.levelExporter != null && window.audioController.Clip != null)
        {
            foreach (LevelObject obj in window.cachedLevelObjects)
            {
                if (obj != null && obj.type == ObstacleType.wave)
                {
                    float timeOfWave = obj.transform.position.z / window.levelExporter.Speed;
                    float waveX = totalWidth * (timeOfWave / window.audioController.Clip.length);
                    if (Mathf.Abs(e.mousePosition.x - waveX) <= CLICK_TOLERANCE) return true;
                }
            }
        }
        return false;
    }
}
#endif
