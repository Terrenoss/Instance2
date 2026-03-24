#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RhythmTimelineUI
{
    public float zoomLevel = 1f;
    public Vector2 scrollPosition;
    public float waveformContrast = 1.5f;

    public RhythmTimelineInteraction interaction = new RhythmTimelineInteraction();

    public RhythmTimelineUI()
    {
    }

    public void DrawTimeline(IRhythmEditorContext context, Rect position)
    {
        AudioClip audioClip = context.AudioController.Clip;
        RhythmAudioController audioController = context.AudioController;
        
        if (audioClip != null && audioController.LastProcessedClip != audioClip) 
            audioController.CacheAudioSamples();

        float totalWidth = position.width * zoomLevel;
        if (totalWidth < position.width) totalWidth = position.width;

        Rect scrollViewRect = GUILayoutUtility.GetRect(position.width - 20f, 140);
        Rect contentRect = new Rect(0, 0, totalWidth, 100);

        interaction.HandleScrollAndZoom(context, scrollViewRect, totalWidth, position.width, ref zoomLevel, ref scrollPosition);

        scrollPosition = GUI.BeginScrollView(scrollViewRect, scrollPosition, contentRect);

        RhythmTimelineRenderer.DrawBackground(scrollPosition, position.width);
        RhythmTimelineRenderer.DrawWaveform(context, totalWidth, scrollPosition, position.width, waveformContrast);
        
        bool isHoveringWave = interaction.IsHoveringWave(context, totalWidth, Event.current);
        RhythmTimelineRenderer.DrawZones(context, totalWidth, isHoveringWave);

        interaction.HandleWaveDrag(context, totalWidth);
        interaction.HandleZoneDrag(context, totalWidth);
        
        RhythmTimelineRenderer.DrawPlayhead(context, totalWidth);
        RhythmTimelineRenderer.DrawWaves(context, totalWidth);
        
        interaction.HandleClicks(context, contentRect, totalWidth);

        GUI.EndScrollView();
    }
}
#endif
