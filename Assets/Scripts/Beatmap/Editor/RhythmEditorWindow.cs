#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RhythmEditorWindow : EditorWindow
{
    public LevelExporter levelExporter;
    public GameObject wavePrefab; 
    public GameObject cubePrefab;
    
    public RhythmAudioController audioController;
    public RhythmTimelineUI timelineUI;
    
    public bool isRecording = false; 
    public bool showHelp = false; 
    
    public List<LevelObject> selectedWaves = new List<LevelObject>();
    public List<FrequencyZone> zones = new List<FrequencyZone>();
    public List<FrequencyZone> selectedZones = new List<FrequencyZone>(); 

    public LevelObject[] cachedLevelObjects = new LevelObject[0];
    private Vector2 mainScrollPos;

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
        if (timelineUI == null) timelineUI = new RhythmTimelineUI(this);
        
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
        GameObject container = GameObject.Find("LevelDesign");
        if (container != null) cachedLevelObjects = container.GetComponentsInChildren<LevelObject>(true);
        else cachedLevelObjects = new LevelObject[0];
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
        if (timelineUI == null || audioController == null) return;
        
        timelineUI.interaction.HandleGlobalShortcuts(timelineUI);

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
                
                Transform parent = GameObject.Find("LevelDesign")?.transform; 
                if (parent != null) newWave.transform.SetParent(parent);

                Undo.RegisterCreatedObjectUndo(newWave, "Spawn Wave"); 
                GUI.FocusControl(null); Event.current.Use(); return;
            }
        }

        mainScrollPos = GUILayout.BeginScrollView(mainScrollPos);

        RhythmInspectorUI.DrawHeader(ref showHelp);
        RhythmInspectorUI.DrawSettings(this);
        RhythmInspectorUI.DrawAudioPlayer(this);

        GUILayout.Label("3. Timeline", EditorStyles.boldLabel);
        if (audioController.Clip != null)
        {
            timelineUI.waveformContrast = EditorGUILayout.Slider("Waveform Contrast", timelineUI.waveformContrast, 1f, 20f);
            timelineUI.DrawTimeline(position);
            
            GUILayout.Space(5);
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            float newTime = EditorGUILayout.Slider("Time (sec)", audioController.CurrentTime, 0f, audioController.Clip.length);
            GUILayout.Label(System.TimeSpan.FromSeconds(audioController.CurrentTime).ToString(@"mm\:ss\.ff"), EditorStyles.boldLabel, GUILayout.Width(60));
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck()) audioController.SetAudioTime(newTime);
        }
        GUILayout.Space(20);

        RhythmInspectorUI.DrawWaveInspector(this);
        RhythmInspectorUI.DrawZoneInspector(this);
        RhythmInspectorUI.DrawStatsAndCleanup(this);
        RhythmInspectorUI.DrawProceduralSection(this);
        RhythmInspectorUI.DrawActions(this);

        GUILayout.EndScrollView();
    }
}
#endif