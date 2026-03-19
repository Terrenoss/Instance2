#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RhythmEditorWindow : EditorWindow
{
    private AudioClip audioClip;
    public LevelExporter levelExporter;
    public GameObject wavePrefab; 
    
    private GameObject hiddenAudioPlayer;
    private AudioSource audioSource;
    
    private float currentTime;
    private float volume = 0.5f;
    private bool isRecording = false; 
    
    private float waveformContrast = 1.5f;
    private LevelObject selectedWave = null;

    private float zoomLevel = 1f;
    private Vector2 scrollPosition;
    private bool autoScroll = true;

    private Texture2D waveformTexture;
    private AudioClip lastProcessedClip;

    [MenuItem("Tools/Rhythm Editor")]
    public static void ShowWindow(LevelExporter exporter = null)
    {
        RhythmEditorWindow window = GetWindow<RhythmEditorWindow>("Rhythm Editor");
        if (exporter != null) window.levelExporter = exporter;
        window.Show();
    }

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
        GameObject oldPlayer = GameObject.Find("Hidden_Rhythm_AudioPlayer");
        if (oldPlayer != null) DestroyImmediate(oldPlayer);
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
        if (hiddenAudioPlayer != null) DestroyImmediate(hiddenAudioPlayer);
        if (waveformTexture != null) DestroyImmediate(waveformTexture);
    }

    private void OnEditorUpdate()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            currentTime = audioSource.time;
            Repaint();
        }
    }

    private void OnGUI()
    {
        // --- ÉCOUTE DE LA TOUCHE ESPACE (MODE RECORD) ---
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
        {
            if (isRecording && audioSource != null && audioSource.isPlaying && levelExporter != null && wavePrefab != null)
            {
                float zPos = currentTime * levelExporter.Speed;
                GameObject newWave = (GameObject)PrefabUtility.InstantiatePrefab(wavePrefab);
                newWave.transform.position = new Vector3(0, 0, zPos);
                
                Transform parent = GameObject.Find("LevelDesign")?.transform; 
                if (parent != null) newWave.transform.SetParent(parent);

                Undo.RegisterCreatedObjectUndo(newWave, "Spawn Wave"); 
                Event.current.Use(); 
            }
        }

        GUILayout.Label("Rhythm Editor", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // --- Section 1: Settings ---
        GUILayout.Label("1. Settings", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", audioClip, typeof(AudioClip), false);
        if (EditorGUI.EndChangeCheck())
        {
            currentTime = 0f;
            GenerateWaveformTexture();
            if (audioSource != null) audioSource.clip = audioClip;
        }

        wavePrefab = (GameObject)EditorGUILayout.ObjectField("Wave Prefab", wavePrefab, typeof(GameObject), false);
        GUILayout.Space(15);

        // --- Section 2: Audio Player & Record ---
        GUILayout.Label("2. Audio Player", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        volume = EditorGUILayout.Slider("Volume", volume, 0f, 1f);
        if (EditorGUI.EndChangeCheck() && audioSource != null)
        {
            audioSource.volume = Mathf.Pow(volume, 3);
        }

        GUILayout.BeginHorizontal();
        bool isPlaying = audioSource != null && audioSource.isPlaying;
        if (isPlaying)
        {
            if (GUILayout.Button("⏸ Pause", GUILayout.Height(30))) PauseAudio();
        }
        else
        {
            if (GUILayout.Button("▶ Play", GUILayout.Height(30))) PlayAudio();
        }

        GUI.backgroundColor = isRecording ? new Color(1f, 0.3f, 0.3f) : Color.white;
        if (GUILayout.Button(isRecording ? "🔴 RECORDING ACTIVE" : "⚪ Record Mode", GUILayout.Height(30)))
        {
            isRecording = !isRecording;
        }
        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("⏹ Stop", GUILayout.Height(30))) StopAudio();
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        // --- Section 3: TIMELINE & WAVEFORM ---
        GUILayout.Label("3. Timeline (Ctrl+Molette pour Zoomer)", EditorStyles.boldLabel);
        
        if (audioClip != null)
        {
            EditorGUI.BeginChangeCheck();
            waveformContrast = EditorGUILayout.Slider("Waveform Contrast", waveformContrast, 1f, 8f);
            if (EditorGUI.EndChangeCheck()) GenerateWaveformTexture();

            // SUPPRESSION DES BOUTONS ZOOM ET AUTO-SCROLL DE L'INTERFACE

            DrawWaveform();
            
            GUILayout.Space(5);
            
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            currentTime = EditorGUILayout.Slider("Time (sec)", currentTime, 0f, audioClip.length);
            GUILayout.Label(System.TimeSpan.FromSeconds(currentTime).ToString(@"mm\:ss\.ff"), EditorStyles.boldLabel, GUILayout.Width(60));
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck()) SetAudioTime(currentTime);
        }

        GUILayout.Space(20);

        // --- MINI INSPECTEUR ---
        if (selectedWave != null)
        {
            GUILayout.Label("Selected Wave Properties", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            selectedWave.waveType = (WaveTypeSelection)EditorGUILayout.EnumPopup("Wave Type", selectedWave.waveType);
            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(selectedWave);
            
            GUILayout.Space(5);
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("🗑️ Delete Selected Wave", GUILayout.Height(25)))
            {
                Undo.DestroyObjectImmediate(selectedWave.gameObject);
                selectedWave = null;
                GUI.backgroundColor = Color.white;
                GUIUtility.ExitGUI();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.Space(20);
        }

        // --- Section 4: Actions ---
        GUILayout.Label("4. Actions", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Export Level to JSON", GUILayout.Height(40)))
        {
            if (levelExporter != null) levelExporter.ExportLevel();
            else Debug.LogWarning("Veuillez assigner le Level Exporter !");
        }
        GUI.backgroundColor = Color.white;
    }

    // ==========================================
    // MÉTHODES POUR LE SPECTRE (WAVEFORM)
    // ==========================================
    private void DrawWaveform()
    {
        if (waveformTexture == null || lastProcessedClip != audioClip) GenerateWaveformTexture();

        float totalWidth = waveformTexture.width;
        if (totalWidth < position.width) totalWidth = position.width;

        Rect scrollViewRect = GUILayoutUtility.GetRect(position.width, 130);
        Rect contentRect = new Rect(0, 0, totalWidth, 100);

        // --- GESTION DE LA SOURIS (ZOOM ET DÉFILEMENT) ---
        Event e = Event.current;
        if (e.type == EventType.ScrollWheel && scrollViewRect.Contains(e.mousePosition))
        {
            if (e.control || e.command) 
            {
                // 1. ZOOMER (Ctrl + Molette)
                float timeAtMouse = (scrollPosition.x + e.mousePosition.x) / totalWidth;

                zoomLevel -= e.delta.y * 0.2f; 
                zoomLevel = Mathf.Clamp(zoomLevel, 1f, 10f);
                GenerateWaveformTexture(); 

                float newTotalWidth = waveformTexture.width;
                if (newTotalWidth < position.width) newTotalWidth = position.width;

                scrollPosition.x = (timeAtMouse * newTotalWidth) - e.mousePosition.x;
            }
            else
            {
                // 2. DÉFILEMENT (Molette seule)
                scrollPosition.x += e.delta.y * 50f; 
                scrollPosition.x += e.delta.x * 50f;

                autoScroll = false; // On désactive l'auto-scroll pour te laisser regarder
            }

            e.Use(); 
            Repaint();
        }

        // Auto-Scroll
        if (autoScroll && audioSource != null && audioSource.isPlaying)
        {
            float currentPlayheadX = totalWidth * (currentTime / audioClip.length);
            scrollPosition.x = currentPlayheadX - (scrollViewRect.width / 2f);
        }

        scrollPosition = GUI.BeginScrollView(scrollViewRect, scrollPosition, contentRect);

        GUI.DrawTexture(contentRect, waveformTexture);

        float progress = currentTime / audioClip.length;
        float playheadX = totalWidth * progress;
        EditorGUI.DrawRect(new Rect(playheadX, 0, 2, 100), Color.red);
        EditorGUI.DrawRect(new Rect(playheadX - 4, 0, 10, 10), Color.red);

        // --- DESSIN DES ONDES ---
        if (levelExporter != null)
        {
            LevelObject[] objects = FindObjectsOfType<LevelObject>();
            foreach (LevelObject obj in objects)
            {
                if (obj.type == ObstacleType.wave)
                {
                    float timeOfWave = obj.transform.position.z / levelExporter.Speed;
                    float waveProgress = timeOfWave / audioClip.length;
                    float waveX = totalWidth * waveProgress;
                    
                    if (waveX >= 0 && waveX <= totalWidth)
                    {
                        if (obj == selectedWave)
                        {
                            EditorGUI.DrawRect(new Rect(waveX - 1, 0, 4, 100), Color.green);
                        }
                        else
                        {
                            EditorGUI.DrawRect(new Rect(waveX, 0, 2, 100), Color.cyan);
                        }

                        if (e.type == EventType.MouseDown && contentRect.Contains(e.mousePosition))
                        {
                            if (Mathf.Abs(e.mousePosition.x - waveX) <= 4f)
                            {
                                selectedWave = obj;
                                Selection.activeGameObject = obj.gameObject;
                                e.Use();
                            }
                        }
                    }
                }
            }
        }

        // Clic sur la Timeline pour changer le temps
        if (contentRect.Contains(e.mousePosition) && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
        {
            float clickProgress = e.mousePosition.x / totalWidth;
            SetAudioTime(Mathf.Clamp(clickProgress * audioClip.length, 0f, audioClip.length));
            e.Use();
        }

        GUI.EndScrollView();
    }

    private void GenerateWaveformTexture()
    {
        if (audioClip == null) return;
        
        lastProcessedClip = audioClip;
        int width = (int)(position.width * zoomLevel); 
        if (width < 500) width = 500;
        int height = 100;
        
        waveformTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        
        Color[] bgPixels = new Color[width * height];
        for(int i = 0; i < bgPixels.Length; i++) bgPixels[i] = new Color(0.15f, 0.15f, 0.15f, 1f);
        waveformTexture.SetPixels(bgPixels);

        float[] samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);

        int packSize = (samples.Length / width) + 1;
        Color waveColor = new Color(1f, 0.6f, 0f, 1f);

        for (int i = 0; i < width; i++)
        {
            float max = 0;
            int startIndex = i * packSize;
            int endIndex = Mathf.Min(startIndex + packSize, samples.Length);
            
            for (int j = startIndex; j < endIndex; j++)
            {
                if (Mathf.Abs(samples[j]) > max) max = Mathf.Abs(samples[j]);
            }

            float displayMax = Mathf.Pow(max, waveformContrast); 

            int halfHeight = height / 2;
            int waveHeight = (int)(displayMax * halfHeight);
            
            for (int y = halfHeight - waveHeight; y <= halfHeight + waveHeight; y++)
            {
                waveformTexture.SetPixel(i, y, waveColor);
            }
        }
        waveformTexture.Apply();
    }

    // ==========================================
    // MÉTHODES AUDIO ET SYNCHRO
    // ==========================================
    private void SetAudioTime(float time)
    {
        currentTime = time;
        if (audioSource != null) audioSource.time = currentTime;
        Repaint();
    }

    private void PlayAudio()
    {
        if (audioClip == null) return;

        // On réactive l'auto-scroll quand on lance la musique !
        autoScroll = true; 

        if (hiddenAudioPlayer == null)
        {
            hiddenAudioPlayer = new GameObject("Hidden_Rhythm_AudioPlayer");
            hiddenAudioPlayer.hideFlags = HideFlags.HideAndDontSave;
            audioSource = hiddenAudioPlayer.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        audioSource.clip = audioClip;
        audioSource.volume = Mathf.Pow(volume, 3);
        audioSource.time = currentTime;
        
        if (!audioSource.isPlaying) audioSource.Play();
    }

    private void PauseAudio()
    {
        if (audioSource != null && audioSource.isPlaying) audioSource.Pause();
    }

    private void StopAudio()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            SetAudioTime(0f);
        }
    }
}
#endif