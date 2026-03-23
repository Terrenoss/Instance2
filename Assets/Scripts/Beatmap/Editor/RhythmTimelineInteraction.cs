#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RhythmTimelineInteraction
{
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

    public RhythmTimelineInteraction()
    {
    }

    private float GetTimeAtMouse(float mouseX, float scrollX, float totalWidth, float clipLength)
    {
        float virtualMouseX = mouseX + scrollX;
        return (virtualMouseX / totalWidth) * clipLength;
    }

    public void HandleGlobalShortcuts(IRhythmEditorContext context, ref float zoomLevel, ref Vector2 scrollPosition)
    {
        if (Event.current.type == EventType.KeyDown && GUIUtility.keyboardControl == 0)
        {
            if (Event.current.keyCode == KeyCode.R)
            {
                zoomLevel = MIN_ZOOM;
                scrollPosition.x = 0f;
                Event.current.Use();
                context.RepaintWindow();
            }
            else if (Event.current.keyCode == KeyCode.LeftArrow || Event.current.keyCode == KeyCode.RightArrow)
            {
                float nudge = (Event.current.keyCode == KeyCode.RightArrow) ? NUDGE_AMOUNT : -NUDGE_AMOUNT;
                if (Event.current.shift) nudge *= 5f; 

                if (context.SelectedWaves.Count > 0 && context.LevelExporter != null)
                {
                    float deltaZ = nudge * context.LevelExporter.Speed;
                    foreach (var wave in context.SelectedWaves)
                    {
                        Undo.RecordObject(wave.transform, "Nudge Wave");
                        wave.transform.position += new Vector3(0, 0, deltaZ);
                    }
                    Event.current.Use();
                    context.RepaintWindow();
                }
                else if (context.SelectedZones.Count > 0)
                {
                    foreach (var zone in context.SelectedZones)
                    {
                        float duration = zone.endTime - zone.startTime;
                        float maxStart = (context.AudioController.Clip != null) ? context.AudioController.Clip.length - duration : 999f;
                        
                        zone.startTime = Mathf.Clamp(zone.startTime + nudge, 0f, maxStart);
                        zone.endTime = zone.startTime + duration;
                    }
                    Event.current.Use();
                    context.RepaintWindow();
                }
            }
        }
    }

    public void HandleScrollAndZoom(IRhythmEditorContext context, Rect scrollViewRect, float totalWidth, float positionWidth, ref float zoomLevel, ref Vector2 scrollPosition)
    {
        Event e = Event.current;
        if (e.type == EventType.ScrollWheel && scrollViewRect.Contains(e.mousePosition))
        {
            if (e.control || e.command) 
            {
                float timeAtMouse = (scrollPosition.x + e.mousePosition.x) / totalWidth;
                zoomLevel -= e.delta.y * ZOOM_SPEED; 
                zoomLevel = Mathf.Clamp(zoomLevel, MIN_ZOOM, MAX_ZOOM);

                float newTotalWidth = positionWidth * zoomLevel;
                if (newTotalWidth < positionWidth) newTotalWidth = positionWidth;

                scrollPosition.x = (timeAtMouse * newTotalWidth) - e.mousePosition.x;
            }
            else
            {
                scrollPosition.x += e.delta.y * 50f; 
                scrollPosition.x += e.delta.x * 50f;
                context.AudioController.autoScroll = false;
            }

            e.Use(); 
            context.RepaintWindow();
        }
    }

    public void HandleWaveDrag(IRhythmEditorContext context, float totalWidth, float scrollPositionX)
    {
        Event e = Event.current;
        var audioClip = context.AudioController.Clip;
        
        if (e.type == EventType.MouseUp)
        {
            isDraggingWave = false;
            draggingZone = null;
            isDraggingZoneBody = false;
        }

        if (isDraggingWave && context.SelectedWaves.Count > 0 && e.type == EventType.MouseDrag)
        {
            float timeAtMouse = GetTimeAtMouse(e.mousePosition.x, scrollPositionX, totalWidth, audioClip.length);
            float newTime = Mathf.Clamp(timeAtMouse - dragOffsetTime, 0f, audioClip.length);
            float newZPos = newTime * context.LevelExporter.Speed;
            
            LevelObject active = Selection.activeGameObject?.GetComponent<LevelObject>();
            if (active != null)
            {
                float deltaZ = newZPos - active.transform.position.z;
                foreach (var wave in context.SelectedWaves)
                {
                    Undo.RecordObject(wave.transform, "Move Wave");
                    wave.transform.position = new Vector3(wave.transform.position.x, wave.transform.position.y, wave.transform.position.z + deltaZ);
                }
            }
            
            e.Use();
            context.RepaintWindow();
        }
    }

    public void HandleZoneDrag(IRhythmEditorContext context, float totalWidth, float scrollPositionX)
    {
        Event e = Event.current;
        if (draggingZone != null && e.type == EventType.MouseDrag)
        {
            var audioClip = context.AudioController.Clip;
            float timeAtMouse = GetTimeAtMouse(e.mousePosition.x, scrollPositionX, totalWidth, audioClip.length);

            if (isDraggingZoneBody)
            {
                float newStartTime = timeAtMouse - dragZoneOffsetTime;
                float delta = newStartTime - draggingZone.startTime;

                if (context.SelectedZones.Contains(draggingZone) && context.SelectedZones.Count > 1)
                {
                    float minStart = 0f;
                    foreach (var z in context.SelectedZones) if (z.startTime + delta < minStart) minStart = z.startTime + delta;
                    if (minStart < 0f) delta -= minStart; 

                    foreach (var z in context.SelectedZones)
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
            context.RepaintWindow();
        }
    }

    public void HandleClicks(IRhythmEditorContext context, Rect contentRect, float totalWidth, float scrollPositionX)
    {
        Event e = Event.current;
        if (!contentRect.Contains(e.mousePosition) || isDraggingWave || draggingZone != null) return;
        if (e.type != EventType.MouseDown && e.type != EventType.MouseDrag) return;

        if (TryHandleWaveClick(context, totalWidth)) return;
        if (TryHandleZoneClick(context, totalWidth, scrollPositionX)) return;
        
        HandleTimelineBackgroundClick(context, totalWidth);
    }

    private bool TryHandleWaveClick(IRhythmEditorContext context, float totalWidth)
    {
        if (context.LevelExporter == null) return false;
        if (Event.current.type != EventType.MouseDown) return false;

        Event e = Event.current;
        var audioClip = context.AudioController.Clip;

        foreach (LevelObject obj in context.CachedLevelObjects)
        {
            if (obj == null || obj.type != ObstacleType.wave) continue;
            
            float timeOfWave = obj.transform.position.z / context.LevelExporter.Speed;
            float waveX = totalWidth * (timeOfWave / audioClip.length);
            
            if (waveX >= 0 && waveX <= totalWidth && Mathf.Abs(e.mousePosition.x - waveX) <= CLICK_TOLERANCE)
            {
                context.SelectedZones.Clear(); 
                
                if (e.shift || EditorGUI.actionKey)
                {
                    if (context.SelectedWaves.Contains(obj)) context.SelectedWaves.Remove(obj);
                    else context.SelectedWaves.Add(obj);
                }
                else if (!context.SelectedWaves.Contains(obj))
                {
                    context.SelectedWaves.Clear();
                    context.SelectedWaves.Add(obj);
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

    private bool TryHandleZoneClick(IRhythmEditorContext context, float totalWidth, float scrollPositionX)
    {
        if (Event.current.type != EventType.MouseDown) return false;

        Event e = Event.current;
        var audioClip = context.AudioController.Clip;

        for (int i = context.Zones.Count - 1; i >= 0; i--)
        {
            var zone = context.Zones[i];
            float startXZone = totalWidth * (zone.startTime / audioClip.length);
            float endXZone = totalWidth * (zone.endTime / audioClip.length);

            if (Mathf.Abs(e.mousePosition.x - startXZone) <= CLICK_TOLERANCE)
            {
                draggingZone = zone; isDraggingZoneStart = true;
                HandleZoneSelection(context, zone, e); e.Use(); return true;
            }
            else if (Mathf.Abs(e.mousePosition.x - endXZone) <= CLICK_TOLERANCE)
            {
                draggingZone = zone; isDraggingZoneStart = false;
                HandleZoneSelection(context, zone, e); e.Use(); return true;
            }
            else if (e.mousePosition.x > startXZone && e.mousePosition.x < endXZone)
            {
                draggingZone = zone; isDraggingZoneBody = true;
                HandleZoneSelection(context, zone, e);
                
                dragZoneOffsetTime = GetTimeAtMouse(e.mousePosition.x, scrollPositionX, totalWidth, audioClip.length) - zone.startTime;
                e.Use(); return true;
            }
        }
        return false;
    }

    private void HandleTimelineBackgroundClick(IRhythmEditorContext context, float totalWidth)
    {
        Event e = Event.current;
        var audioClip = context.AudioController.Clip;

        float clickProgress = e.mousePosition.x / totalWidth;
        context.AudioController.SetAudioTime(Mathf.Clamp(clickProgress * audioClip.length, 0f, audioClip.length));
        context.RepaintWindow();
        
        if (e.type == EventType.MouseDown)
        {
            context.SelectedWaves.Clear();
            Selection.activeGameObject = null;
            context.SelectedZones.Clear();

            if (e.clickCount == 2) context.AudioController.PlayAudio();
        }
        e.Use();
    }

    private void HandleZoneSelection(IRhythmEditorContext context, FrequencyZone zone, Event e)
    {
        context.SelectedWaves.Clear(); 
        Selection.activeGameObject = null;

        if (e.shift || EditorGUI.actionKey)
        {
            if (context.SelectedZones.Contains(zone)) context.SelectedZones.Remove(zone);
            else context.SelectedZones.Add(zone);
        }
        else if (!context.SelectedZones.Contains(zone))
        {
            context.SelectedZones.Clear();
            context.SelectedZones.Add(zone);
        }
    }

    public bool IsHoveringWave(IRhythmEditorContext context, float totalWidth, Event e)
    {
        if (context.LevelExporter != null && context.AudioController.Clip != null)
        {
            foreach (LevelObject obj in context.CachedLevelObjects)
            {
                if (obj != null && obj.type == ObstacleType.wave)
                {
                    float timeOfWave = obj.transform.position.z / context.LevelExporter.Speed;
                    float waveX = totalWidth * (timeOfWave / context.AudioController.Clip.length);
                    if (Mathf.Abs(e.mousePosition.x - waveX) <= CLICK_TOLERANCE) return true;
                }
            }
        }
        return false;
    }
}
#endif
