#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RhythmTimelineUI
{
    private RhythmEditorWindow window;
    
    public float zoomLevel = 1f;
    public Vector2 scrollPosition;
    public float waveformContrast = 1.5f;

    public RhythmTimelineInteraction interaction;

    public RhythmTimelineUI(RhythmEditorWindow window)
    {
        this.window = window;
        this.interaction = new RhythmTimelineInteraction(window);
    }

    public void DrawTimeline(Rect position)
    {
        var audioClip = window.audioController.Clip;
        var audioController = window.audioController;
        
        if (audioClip != null && audioController.LastProcessedClip != audioClip) 
            audioController.CacheAudioSamples();

        float totalWidth = position.width * zoomLevel;
        if (totalWidth < position.width) totalWidth = position.width;

        Rect scrollViewRect = GUILayoutUtility.GetRect(position.width - 20f, 140);
        Rect contentRect = new Rect(0, 0, totalWidth, 100);

        interaction.HandleWaveDrag(totalWidth, scrollPosition.x);
        interaction.HandleScrollAndZoom(scrollViewRect, totalWidth, position.width, this);

        scrollPosition = GUI.BeginScrollView(scrollViewRect, scrollPosition, contentRect);

        RhythmTimelineRenderer.DrawBackground(scrollPosition, position.width);
        RhythmTimelineRenderer.DrawWaveform(window, totalWidth, scrollPosition, position.width, waveformContrast);
        
        bool isHoveringWave = interaction.IsHoveringWave(totalWidth, Event.current);
        RhythmTimelineRenderer.DrawZones(window, totalWidth, isHoveringWave);

        interaction.HandleZoneDrag(totalWidth, scrollPosition.x);
        
        RhythmTimelineRenderer.DrawPlayhead(window, totalWidth);
        RhythmTimelineRenderer.DrawWaves(window, totalWidth);
        
        interaction.HandleClicks(contentRect, totalWidth, scrollPosition.x);

        GUI.EndScrollView();
    }
}
#endif
