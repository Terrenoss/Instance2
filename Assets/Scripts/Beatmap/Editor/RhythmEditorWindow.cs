#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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

public class RhythmEditorWindow : EditorWindow, IRhythmEditorContext
{
    public RhythmAudioController audioController;
    public LevelExporter levelExporter;
    
    public RhythmTimelineUI timelineUI;
    public GameObject wavePrefab;
    public GameObject cubePrefab;
    
    public bool isRecording = false; 
    public bool showHelp = false; 
    
    public List<LevelObject> selectedWaves = new List<LevelObject>();
    public List<FrequencyZone> selectedZones = new List<FrequencyZone>(); 

    public LevelObject[] cachedLevelObjects = new LevelObject[0];
    private Vector2 mainScrollPos;

    // IRhythmEditorContext Implementation
    public RhythmAudioController AudioController => audioController;
    public LevelExporter LevelExporter => levelExporter;
    public List<LevelObject> SelectedWaves => selectedWaves;
    public List<FrequencyZone> Zones => (levelExporter != null) ? levelExporter.zones : new List<FrequencyZone>();
    public List<FrequencyZone> SelectedZones => selectedZones;
    public LevelObject[] CachedLevelObjects => cachedLevelObjects;
    
    public GameObject WavePrefab { get => wavePrefab; set => wavePrefab = value; }
    public GameObject CubePrefab { get => cubePrefab; set => cubePrefab = value; }
    public bool IsRecording { get => isRecording; set => isRecording = value; }
    public bool ShowHelp { get => showHelp; set => showHelp = value; }
    
    public void RepaintWindow() => Repaint();

    [MenuItem("Tools/Rhythm Editor")]
    public static void ShowWindow()
    {
        RhythmEditorWindow window = GetWindow<RhythmEditorWindow>("Rhythm Editor");
        window.Show();
    }

    public static void ShowWindowWithExporter(LevelExporter exporter)
    {
        RhythmEditorWindow window = GetWindow<RhythmEditorWindow>("Rhythm Editor");
        window.levelExporter = exporter;
        window.Show();
    }

    private void OnEnable()
    {
        if (audioController == null) audioController = new RhythmAudioController();
        if (timelineUI == null) timelineUI = new RhythmTimelineUI();
        
        audioController.OnEnable();
        
        EditorApplication.update += OnEditorUpdate;
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        OnHierarchyChanged();
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        
        if (audioController != null) audioController.OnDisable();
    }

    private void OnHierarchyChanged()
    {
        if (levelExporter != null && levelExporter.BlocksParent != null) 
            cachedLevelObjects = levelExporter.BlocksParent.GetComponentsInChildren<LevelObject>(true);
        else 
            cachedLevelObjects = new LevelObject[0];
            
        Repaint();
    }

    private void OnEditorUpdate()
    {
        if (audioController != null && audioController.IsPlaying())
        {
            audioController.UpdateTimeFromSource();
            Repaint();
        }
    }

    private void OnGUI()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Rhythm Editor is unavailable during Play Mode.", MessageType.Warning);
            return;
        }

        if (audioController == null) audioController = new RhythmAudioController();
        if (timelineUI == null) timelineUI = new RhythmTimelineUI();
        
        if (timelineUI == null || audioController == null) return;
        
        timelineUI.interaction.HandleGlobalShortcuts(this, ref timelineUI.zoomLevel, ref timelineUI.scrollPosition);

        if (Event.current.type == EventType.MouseDown)
        {
            GUI.FocusControl(null); Repaint();
        }

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space && GUIUtility.keyboardControl == 0)
        {
            if (isRecording && levelExporter != null && wavePrefab != null)
            {
                float zPos = audioController.CurrentTime * levelExporter.Speed;
                GameObject newWave = (GameObject)PrefabUtility.InstantiatePrefab(wavePrefab);
                newWave.transform.position = new Vector3(0, 0, zPos);
                
                if (levelExporter != null && levelExporter.BlocksParent != null) 
                    newWave.transform.SetParent(levelExporter.BlocksParent);

                Undo.RegisterCreatedObjectUndo(newWave, "Spawn Wave"); 
                GUI.FocusControl(null); Event.current.Use(); return;
            }
        }

        RhythmInspectorUI.DrawHeader(this);

        mainScrollPos = GUILayout.BeginScrollView(mainScrollPos);

        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.BeginVertical(GUILayout.Width(350));
        RhythmInspectorUI.DrawSettings(this);
        RhythmInspectorUI.DrawAudioPlayer(this);
        RhythmInspectorUI.DrawWaveInspector(this);
        RhythmInspectorUI.DrawZoneInspector(this);
        RhythmInspectorUI.DrawStatsAndCleanup(this);
        RhythmInspectorUI.DrawProceduralSection(this);
        RhythmInspectorUI.DrawActions(this);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        GUILayout.Label("3. Timeline", EditorStyles.boldLabel);
        if (audioController.Clip != null)
        {
            timelineUI.waveformContrast = EditorGUILayout.Slider("Waveform Contrast", timelineUI.waveformContrast, 1f, 20f);
            Rect timelineRect = GUILayoutUtility.GetRect(position.width - 380, 200, GUILayout.ExpandWidth(true));
            timelineUI.DrawTimeline(this, timelineRect);
            
            GUILayout.Space(5);
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            float newTime = EditorGUILayout.Slider("Time (sec)", audioController.CurrentTime, 0f, audioController.Clip.length);
            GUILayout.Label(System.TimeSpan.FromSeconds(audioController.CurrentTime).ToString(@"mm\:ss\.ff"), EditorStyles.boldLabel, GUILayout.Width(60));
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck()) audioController.SetAudioTime(newTime);
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();

        GUILayout.EndScrollView();
    }
}
#endif