#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

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

public class RhythmEditorWindow : EditorWindow
{
    private AudioClip audioClip;
    public LevelExporter levelExporter;
    public GameObject wavePrefab; 
    
    private GameObject hiddenAudioPlayer;
    private AudioSource audioSource;
    
    private float currentTime;
    private float volume = 0.15f;
    private bool isRecording = false; 
    private bool showHelp = false; 
    
    private float waveformContrast = 1.5f;
    private System.Collections.Generic.List<LevelObject> selectedWaves = new System.Collections.Generic.List<LevelObject>();
    
    public GameObject cubePrefab;
    public System.Collections.Generic.List<FrequencyZone> zones = new System.Collections.Generic.List<FrequencyZone>();

    private FrequencyZone draggingZone = null;
    private System.Collections.Generic.List<FrequencyZone> selectedZones = new System.Collections.Generic.List<FrequencyZone>(); 
    private bool isDraggingZoneStart = false;

    private float zoomLevel = 1f;
    private Vector2 scrollPosition;
    private bool autoScroll = true;

    private float[] cachedSamples;
    private AudioClip lastProcessedClip;
    private bool isDraggingWave = false;
    private float dragOffsetTime = 0f;

    private Vector2 mainScrollPos;
    private bool isDraggingZoneBody = false;
    private float dragZoneOffsetTime = 0f;

    [MenuItem("Tools/Rhythm Editor")]
    public static void ShowWindow(LevelExporter exporter = null)
    {
        RhythmEditorWindow window = GetWindow<RhythmEditorWindow>("Rhythm Editor");
        if (exporter != null) window.levelExporter = exporter;
        window.Show();
    }

    private LevelObject[] cachedLevelObjects = new LevelObject[0];

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        GameObject oldPlayer = GameObject.Find("Hidden_Rhythm_AudioPlayer");
        if (oldPlayer != null) DestroyImmediate(oldPlayer);
        
        if (audioClip != null) CacheAudioSamples();
        OnHierarchyChanged();
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        if (hiddenAudioPlayer != null) DestroyImmediate(hiddenAudioPlayer);
    }

    private void OnHierarchyChanged()
    {
        GameObject container = GameObject.Find("LevelDesign");
        if (container != null)
        {
            cachedLevelObjects = container.GetComponentsInChildren<LevelObject>(true);
        }
        else
        {
            cachedLevelObjects = new LevelObject[0];
        }
        Repaint();
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
        // --- RACCOURCIS CLAVIER GLOBAUX ---
        if (Event.current.type == EventType.KeyDown && GUIUtility.keyboardControl == 0)
        {
            if (Event.current.keyCode == KeyCode.R)
            {
                zoomLevel = 1f;
                scrollPosition.x = 0f;
                Event.current.Use();
                Repaint();
            }
            else if (Event.current.keyCode == KeyCode.LeftArrow || Event.current.keyCode == KeyCode.RightArrow)
            {
                float nudge = (Event.current.keyCode == KeyCode.RightArrow) ? 0.01f : -0.01f;
                if (Event.current.shift) nudge *= 5f; 

                if (selectedWaves.Count > 0 && levelExporter != null)
                {
                    float deltaZ = nudge * levelExporter.Speed;
                    foreach (var wave in selectedWaves)
                    {
                        Undo.RecordObject(wave.transform, "Nudge Wave");
                        wave.transform.position += new Vector3(0, 0, deltaZ);
                    }
                    Event.current.Use();
                    Repaint();
                }
                else if (selectedZones.Count > 0)
                {
                    foreach (var zone in selectedZones)
                    {
                        float duration = zone.endTime - zone.startTime;
                        float maxStart = (audioClip != null) ? audioClip.length - duration : 999f;
                        
                        zone.startTime = Mathf.Clamp(zone.startTime + nudge, 0f, maxStart);
                        zone.endTime = zone.startTime + duration;
                    }
                    Event.current.Use();
                    Repaint();
                }
            }
        }

        if (Event.current.type == EventType.MouseDown)
        {
            GUI.FocusControl(null);
            Repaint();
        }

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space && GUIUtility.keyboardControl == 0)
        {
            if (isRecording && levelExporter != null && wavePrefab != null)
            {
                float zPos = currentTime * levelExporter.Speed;
                GameObject newWave = (GameObject)PrefabUtility.InstantiatePrefab(wavePrefab);
                newWave.transform.position = new Vector3(0, 0, zPos);
                
                Transform parent = GameObject.Find("LevelDesign")?.transform; 
                if (parent != null) newWave.transform.SetParent(parent);

                Undo.RegisterCreatedObjectUndo(newWave, "Spawn Wave"); 
                GUI.FocusControl(null);
                Event.current.Use(); 
                return;
            }
        }

        mainScrollPos = GUILayout.BeginScrollView(mainScrollPos);

        // --- EN-TÊTE ET BOUTON AIDE ---
        GUILayout.BeginHorizontal();
        GUILayout.Label("Rhythm Editor", EditorStyles.boldLabel);
        if (GUILayout.Button("❔ Aide / Raccourcis", GUILayout.Width(140)))
        {
            showHelp = !showHelp;
        }
        GUILayout.EndHorizontal();

        if (showHelp)
        {
            EditorGUILayout.HelpBox(
                "RACCOURCIS CLAVIER & SOURIS :\n" +
                "• [Espace] : Placer une onde (uniquement si 'Record Mode' est actif)\n" +
                "• [Double-Clic Timeline] : Lancer la lecture audio d'ici\n" +
                "• [Maj + Clic] : Sélectionner plusieurs Ondes ou plusieurs Zones (Timeline)\n" +
                "• [R] : Réinitialiser le zoom (1x) et revenir au début\n" +
                "• [Flèche Gauche / Droite] : Déplacer les éléments sélectionnés\n" +
                "• [Maj + Flèches] : Déplacer les éléments 5x plus vite", MessageType.Info);
            GUILayout.Space(10);
        }
        
        GUILayout.Space(10);

        // --- Section 1: Settings ---
        GUILayout.Label("1. Settings", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", audioClip, typeof(AudioClip), false);
        if (EditorGUI.EndChangeCheck())
        {
            currentTime = 0f;
            CacheAudioSamples();
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
        GUILayout.Label("3. Timeline (Touche 'R' = Reset Zoom | Flèches = Nudge)", EditorStyles.boldLabel);
        
        if (audioClip != null)
        {
            waveformContrast = EditorGUILayout.Slider("Waveform Contrast", waveformContrast, 1f, 20f);

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

        // --- MINI INSPECTEUR D'ONDES ---
        if (selectedWaves.Count > 0)
        {
            GUILayout.Label($"Selected Waves Properties ({selectedWaves.Count} elements)", EditorStyles.boldLabel);
            
            WaveTypeSelection currentWaveType = selectedWaves[0].waveType;
            bool currentParryVisible = selectedWaves[0].isParryKeyVisible;

            EditorGUI.BeginChangeCheck();
            WaveTypeSelection newWaveType = (WaveTypeSelection)EditorGUILayout.EnumPopup("Wave Type", currentWaveType);
            bool newParryVisible = EditorGUILayout.Toggle("Is Parry Key Visible", currentParryVisible);
            
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var wave in selectedWaves)
                {
                    Undo.RecordObject(wave, "Change Wave Properties");
                    wave.waveType = newWaveType;
                    wave.isParryKeyVisible = newParryVisible;
                    EditorUtility.SetDirty(wave);
                }
            }
            
            GUILayout.Space(5);
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("🗑️ Delete Selected Waves", GUILayout.Height(25)))
            {
                foreach (var wave in selectedWaves)
                {
                    if (wave != null && wave.gameObject != null)
                        Undo.DestroyObjectImmediate(wave.gameObject);
                }
                selectedWaves.Clear();
                GUI.backgroundColor = Color.white;
                GUIUtility.ExitGUI();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.Space(20);
        }

        // --- MINI INSPECTEUR DE ZONES (MULTIPLE SELECTION) ---
        if (selectedZones.Count > 1) 
        {
            GUILayout.Label($"Selected Zones Properties ({selectedZones.Count} elements)", EditorStyles.boldLabel);
            
            float commonProb = selectedZones[0].probability;
            float commonBeat = selectedZones[0].beatInterval;
            float commonLane = selectedZones[0].laneOffset;
            float commonSafety = selectedZones[0].safetyMargin;
            Color commonCol = selectedZones[0].zoneColor;

            EditorGUI.BeginChangeCheck();
            float newProb = EditorGUILayout.Slider("Spawn Density", commonProb, 0f, 1f);
            Color newCol = EditorGUILayout.ColorField("Color", commonCol);
            float newBeat = EditorGUILayout.FloatField("Beat (s)", commonBeat);
            float newLane = EditorGUILayout.FloatField("Lane Offset", commonLane);
            float newSafety = EditorGUILayout.FloatField("Safety (s)", commonSafety);
            
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var z in selectedZones)
                {
                    z.probability = newProb;
                    z.zoneColor = newCol;
                    z.beatInterval = newBeat;
                    z.laneOffset = newLane;
                    z.safetyMargin = newSafety;
                }
            }
            GUILayout.Space(20);
        }

        // --- Section: Stats & Clean Up ---
        GUILayout.Label("Stats & Clean Up", EditorStyles.boldLabel);
        
        int waveCount = 0;
        foreach (var obj in cachedLevelObjects)
        {
            if (obj != null && obj.type == ObstacleType.wave)
                waveCount++;
        }

        int procBlocksCount = 0;
        Transform procBlocksTf = levelExporter != null && levelExporter.BlocksParent != null ? levelExporter.BlocksParent.Find("ProceduralBlocks") : null;
        if (procBlocksTf != null)
            procBlocksCount = procBlocksTf.childCount;

        GUILayout.Label($"Waves count: {waveCount}");
        GUILayout.Label($"Procedural Blocks count: {procBlocksCount}");

        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear All Waves", GUILayout.Height(30)))
        {
            foreach (var obj in cachedLevelObjects)
            {
                if (obj != null && obj.type == ObstacleType.wave)
                {
                    Undo.DestroyObjectImmediate(obj.gameObject);
                }
            }
            selectedWaves.Clear();
        }
        if (GUILayout.Button("Clear Procedural Blocks", GUILayout.Height(30)))
        {
            if (procBlocksTf != null)
            {
                Undo.DestroyObjectImmediate(procBlocksTf.gameObject);
            }
        }
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        // --- Section: Procedural Generation ---
        GUILayout.Label("Procedural Generation", EditorStyles.boldLabel);
        cubePrefab = (GameObject)EditorGUILayout.ObjectField("Cube Prefab", cubePrefab, typeof(GameObject), false);
        
        GUILayout.Space(5);
        GUILayout.Label("Frequency Zones:", EditorStyles.boldLabel);
        
        for (int i = 0; i < zones.Count; i++)
        {
            if (selectedZones.Contains(zones[i])) 
            {
                GUI.backgroundColor = new Color(1f, 1f, 0f, 0.5f); // Fond Jaune Vif
            }
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Zone {i + 1}", EditorStyles.boldLabel, GUILayout.Width(60));
            zones[i].startTime = EditorGUILayout.FloatField(zones[i].startTime, GUILayout.Width(50));
            GUILayout.Label("-", GUILayout.Width(10));
            zones[i].endTime = EditorGUILayout.FloatField(zones[i].endTime, GUILayout.Width(50));
            
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("▲", GUILayout.Width(25)) && i > 0)
            {
                var temp = zones[i]; zones[i] = zones[i - 1]; zones[i - 1] = temp;
            }
            if (GUILayout.Button("▼", GUILayout.Width(25)) && i < zones.Count - 1)
            {
                var temp = zones[i]; zones[i] = zones[i + 1]; zones[i + 1] = temp;
            }
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                if (selectedZones.Contains(zones[i])) selectedZones.Remove(zones[i]);
                zones.RemoveAt(i);
                i--;
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                continue;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            zones[i].probability = EditorGUILayout.Slider("Spawn Density", zones[i].probability, 0f, 1f);
            zones[i].zoneColor = EditorGUILayout.ColorField(zones[i].zoneColor, GUILayout.Width(60));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            zones[i].beatInterval = EditorGUILayout.FloatField("Beat (s)", zones[i].beatInterval);
            zones[i].laneOffset = EditorGUILayout.FloatField("Lane Offset", zones[i].laneOffset);
            zones[i].safetyMargin = EditorGUILayout.FloatField("Safety (s)", zones[i].safetyMargin);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
        
        if (GUILayout.Button("Add Zone")) zones.Add(new FrequencyZone());

        GUILayout.Space(10);
        GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
        if (GUILayout.Button("Generate Procedural Blocks", GUILayout.Height(40)))
        {
            GenerateProceduralBlocks();
        }
        GUI.backgroundColor = Color.white;
        GUILayout.Space(20);

        // --- Section 4: Actions ---
        GUILayout.Label("4. Actions", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Export Level to JSON", GUILayout.Height(40)))
        {
            if (levelExporter != null) levelExporter.ExportLevel();
            else Debug.LogWarning("Veuillez assigner le Level Exporter !");
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(60); 
        GUILayout.EndScrollView();
    }

    // ==========================================
    // MÉTHODES POUR LE SPECTRE (WAVEFORM)
    // ==========================================
    private void CacheAudioSamples()
    {
        if (audioClip != null)
        {
            lastProcessedClip = audioClip;
            cachedSamples = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(cachedSamples, 0);
        }
        else
        {
            cachedSamples = null;
        }
    }

    private void DrawWaveform()
    {
        if (audioClip != null && lastProcessedClip != audioClip) CacheAudioSamples();

        float totalWidth = position.width * zoomLevel;
        if (totalWidth < position.width) totalWidth = position.width;

        Rect scrollViewRect = GUILayoutUtility.GetRect(position.width - 20f, 140);
        Rect contentRect = new Rect(0, 0, totalWidth, 100);

        Event e = Event.current;
        
        if (e.type == EventType.MouseUp)
        {
            isDraggingWave = false;
            draggingZone = null;
            isDraggingZoneBody = false;
        }

        // --- GESTION DU DRAG DES ONDES ---
        if (isDraggingWave && selectedWaves.Count > 0 && e.type == EventType.MouseDrag)
        {
            float virtualMouseX = e.mousePosition.x + scrollPosition.x;
            float timeAtMouse = (virtualMouseX / totalWidth) * audioClip.length;
            float newTime = Mathf.Clamp(timeAtMouse - dragOffsetTime, 0f, audioClip.length);
            float newZPos = newTime * levelExporter.Speed;
            
            LevelObject active = Selection.activeGameObject?.GetComponent<LevelObject>();
            if (active != null)
            {
                float deltaZ = newZPos - active.transform.position.z;
                foreach (var wave in selectedWaves)
                {
                    Undo.RecordObject(wave.transform, "Move Wave");
                    wave.transform.position = new Vector3(wave.transform.position.x, wave.transform.position.y, wave.transform.position.z + deltaZ);
                }
            }
            
            e.Use();
            Repaint();
        }

        if (e.type == EventType.ScrollWheel && scrollViewRect.Contains(e.mousePosition))
        {
            if (e.control || e.command) 
            {
                float timeAtMouse = (scrollPosition.x + e.mousePosition.x) / totalWidth;
                zoomLevel -= e.delta.y * 0.2f; 
                zoomLevel = Mathf.Clamp(zoomLevel, 1f, 100f);

                float newTotalWidth = position.width * zoomLevel;
                if (newTotalWidth < position.width) newTotalWidth = position.width;

                scrollPosition.x = (timeAtMouse * newTotalWidth) - e.mousePosition.x;
            }
            else
            {
                scrollPosition.x += e.delta.y * 50f; 
                scrollPosition.x += e.delta.x * 50f;
                autoScroll = false;
            }

            e.Use(); 
            Repaint();
        }

        scrollPosition = GUI.BeginScrollView(scrollViewRect, scrollPosition, contentRect);

        EditorGUI.DrawRect(new Rect(scrollPosition.x, 0, position.width, 100), new Color(0.15f, 0.15f, 0.15f, 1f));

        if (Event.current.type == EventType.Repaint && cachedSamples != null && audioClip != null)
        {
            Color waveColor = new Color(1f, 0.6f, 0f, 1f);
            float startX = scrollPosition.x;
            float endX = startX + position.width;
            float unitsPerPixel = audioClip.length / totalWidth; 
            int sampleRate = audioClip.frequency * audioClip.channels;

            for (float x = startX; x <= endX; x++)
            {
                float timeStart = x * unitsPerPixel;
                float timeEnd = (x + 1) * unitsPerPixel;

                int sStart = Mathf.Clamp(Mathf.FloorToInt(timeStart * sampleRate), 0, cachedSamples.Length);
                int sEnd = Mathf.Clamp(Mathf.FloorToInt(timeEnd * sampleRate), 0, cachedSamples.Length);
                int count = sEnd - sStart;

                float max = 0;
                if (count > 0) 
                {
                    for (int j = sStart; j < sEnd; j++)
                    {
                        float val = Mathf.Abs(cachedSamples[j]);
                        if (val > max) max = val;
                    }
                }
                
                float displayMax = Mathf.Pow(max, waveformContrast);
                float h = displayMax * 50f;
                EditorGUI.DrawRect(new Rect(x, 50 - h, 1, h * 2), waveColor);
            }
        }

        // --- CALCUL PRIORITÉ DU CURSEUR (Souris sur une onde ?) ---
        bool isHoveringWave = false;
        if (levelExporter != null && e.type == EventType.Repaint)
        {
            foreach (LevelObject obj in cachedLevelObjects)
            {
                if (obj != null && obj.type == ObstacleType.wave)
                {
                    float timeOfWave = obj.transform.position.z / levelExporter.Speed;
                    float waveX = totalWidth * (timeOfWave / audioClip.length);
                    if (Mathf.Abs(e.mousePosition.x - waveX) <= 5f)
                    {
                        isHoveringWave = true;
                        break;
                    }
                }
            }
        }

        // 1. DESSIN DES ZONES
        foreach (var zone in zones)
        {
            float startXZone = totalWidth * (zone.startTime / audioClip.length);
            float endXZone = totalWidth * (zone.endTime / audioClip.length);
            
            EditorGUI.DrawRect(new Rect(startXZone, 0, endXZone - startXZone, 100), zone.zoneColor);
            
            // Couleur opposée si la zone est sélectionnée
            Color solidColor = selectedZones.Contains(zone) 
                ? new Color(1f - zone.zoneColor.r, 1f - zone.zoneColor.g, 1f - zone.zoneColor.b, 1f) 
                : new Color(zone.zoneColor.r, zone.zoneColor.g, zone.zoneColor.b, 1f);

            EditorGUI.DrawRect(new Rect(startXZone, 0, endXZone - startXZone, 4), solidColor);
            EditorGUI.DrawRect(new Rect(startXZone, 96, endXZone - startXZone, 4), solidColor);

            // N'ajouter le curseur "Pan" (main) que si l'on ne survole PAS une onde
            if (!isHoveringWave)
            {
                EditorGUIUtility.AddCursorRect(new Rect(startXZone - 5, 0, 10, 100), MouseCursor.ResizeHorizontal);
                EditorGUIUtility.AddCursorRect(new Rect(endXZone - 5, 0, 10, 100), MouseCursor.ResizeHorizontal);
                EditorGUIUtility.AddCursorRect(new Rect(startXZone + 5, 0, (endXZone - startXZone) - 10, 100), MouseCursor.Pan);
            }
        }

        // --- GESTION DU DRAG DES ZONES ---
        if (draggingZone != null && e.type == EventType.MouseDrag)
        {
            float virtualMouseX = e.mousePosition.x + scrollPosition.x;
            float timeAtMouse = (virtualMouseX / totalWidth) * audioClip.length;

            if (isDraggingZoneBody)
            {
                float newStartTime = timeAtMouse - dragZoneOffsetTime;
                float delta = newStartTime - draggingZone.startTime;

                // Si on bouge une zone faisant partie d'une sélection multiple
                if (selectedZones.Contains(draggingZone) && selectedZones.Count > 1)
                {
                    // Empêcher d'aller en dessous de zéro
                    float minStart = 0f;
                    foreach (var z in selectedZones) if (z.startTime + delta < minStart) minStart = z.startTime + delta;
                    if (minStart < 0f) delta -= minStart; 

                    foreach (var z in selectedZones)
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
            Repaint();
        }

        float progress = currentTime / audioClip.length;
        float playheadX = totalWidth * progress;
        
        EditorGUI.DrawRect(new Rect(playheadX, 0, 2, 100), Color.red);
        EditorGUI.DrawRect(new Rect(playheadX - 4, 0, 10, 10), Color.red);

        // 2. DESSIN ET DETECTION DE CLIC POUR LES ONDES (Priorité Absolue)
        if (levelExporter != null)
        {
            foreach (LevelObject obj in cachedLevelObjects)
            {
                if (obj == null) continue;
                
                if (obj.type == ObstacleType.wave)
                {
                    float timeOfWave = obj.transform.position.z / levelExporter.Speed;
                    float waveProgress = timeOfWave / audioClip.length;
                    float waveX = totalWidth * waveProgress;
                    
                    if (waveX >= 0 && waveX <= totalWidth)
                    {
                        if (selectedWaves.Contains(obj))
                            EditorGUI.DrawRect(new Rect(waveX - 1, 0, 4, 100), Color.green);
                        else
                            EditorGUI.DrawRect(new Rect(waveX, 0, 2, 100), Color.cyan);

                        // FIX : Force l'icône de la petite main cliquable "Link" sur les Ondes !
                        EditorGUIUtility.AddCursorRect(new Rect(waveX - 3, 0, 6, 100), MouseCursor.Link);

                        if (!isDraggingWave && draggingZone == null && e.type == EventType.MouseDown && contentRect.Contains(e.mousePosition))
                        {
                            if (Mathf.Abs(e.mousePosition.x - waveX) <= 4f)
                            {
                                selectedZones.Clear(); 
                                
                                if (e.shift || EditorGUI.actionKey)
                                {
                                    if (selectedWaves.Contains(obj)) selectedWaves.Remove(obj);
                                    else selectedWaves.Add(obj);
                                }
                                else
                                {
                                    if (!selectedWaves.Contains(obj))
                                    {
                                        selectedWaves.Clear();
                                        selectedWaves.Add(obj);
                                    }
                                }

                                Selection.activeGameObject = obj.gameObject;
                                isDraggingWave = true;
                                
                                float clickTime = (e.mousePosition.x / totalWidth) * audioClip.length;
                                dragOffsetTime = clickTime - timeOfWave;
                                
                                if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.FrameSelected();
                                
                                e.Use(); 
                            }
                        }
                    }
                }
            }
        }

        // 3. DETECTION DE CLIC POUR LES ZONES
        if (e.type == EventType.MouseDown && contentRect.Contains(e.mousePosition) && !isDraggingWave && draggingZone == null)
        {
            for (int i = zones.Count - 1; i >= 0; i--)
            {
                var zone = zones[i];
                float startXZone = totalWidth * (zone.startTime / audioClip.length);
                float endXZone = totalWidth * (zone.endTime / audioClip.length);

                if (Mathf.Abs(e.mousePosition.x - startXZone) <= 5f)
                {
                    draggingZone = zone;
                    isDraggingZoneStart = true;
                    HandleZoneSelection(zone, e);
                    e.Use(); break;
                }
                else if (Mathf.Abs(e.mousePosition.x - endXZone) <= 5f)
                {
                    draggingZone = zone;
                    isDraggingZoneStart = false;
                    HandleZoneSelection(zone, e);
                    e.Use(); break;
                }
                else if (e.mousePosition.x > startXZone && e.mousePosition.x < endXZone)
                {
                    draggingZone = zone;
                    isDraggingZoneBody = true;
                    HandleZoneSelection(zone, e);
                    
                    float virtualMouseX = e.mousePosition.x + scrollPosition.x;
                    float timeAtMouse = (virtualMouseX / totalWidth) * audioClip.length;
                    dragZoneOffsetTime = timeAtMouse - zone.startTime;
                    e.Use(); break;
                }
            }
        }

        // 4. DETECTION DE CLIC DANS LE VIDE (Timeline)
        if (contentRect.Contains(e.mousePosition) && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && !isDraggingWave && draggingZone == null)
        {
            float clickProgress = e.mousePosition.x / totalWidth;
            SetAudioTime(Mathf.Clamp(clickProgress * audioClip.length, 0f, audioClip.length));
            
            if (e.type == EventType.MouseDown)
            {
                selectedWaves.Clear();
                Selection.activeGameObject = null;
                selectedZones.Clear();

                if (e.clickCount == 2) PlayAudio();
            }
            e.Use();
        }

        GUI.EndScrollView();
    }

    // Gestion de la sélection (Simple / Multiple) des Zones
    private void HandleZoneSelection(FrequencyZone zone, Event e)
    {
        selectedWaves.Clear(); 
        Selection.activeGameObject = null;

        if (e.shift || EditorGUI.actionKey)
        {
            if (selectedZones.Contains(zone)) selectedZones.Remove(zone);
            else selectedZones.Add(zone);
        }
        else
        {
            if (!selectedZones.Contains(zone))
            {
                selectedZones.Clear();
                selectedZones.Add(zone);
            }
        }
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

    private void GenerateProceduralBlocks()
    {
        if (levelExporter == null || levelExporter.BlocksParent == null || cubePrefab == null)
        {
            Debug.LogWarning("Missing references for procedural generation.");
            return;
        }

        Transform blocksParent = levelExporter.BlocksParent;
        Transform procBlocksTf = blocksParent.Find("ProceduralBlocks");
        if (procBlocksTf != null) Undo.DestroyObjectImmediate(procBlocksTf.gameObject);

        GameObject procBlocksGO = new GameObject("ProceduralBlocks");
        procBlocksGO.transform.SetParent(blocksParent);
        Undo.RegisterCreatedObjectUndo(procBlocksGO, "Create ProceduralBlocks");
        procBlocksTf = procBlocksGO.transform;

        System.Collections.Generic.List<float> waveTimes = new System.Collections.Generic.List<float>();
        foreach (LevelObject obj in cachedLevelObjects)
        {
            if (obj != null && obj.type == ObstacleType.wave)
            {
                waveTimes.Add(obj.transform.position.z / levelExporter.Speed);
            }
        }

        foreach (var zone in zones)
        {
            if (zone.probability <= 0f) continue;

            for (float t = zone.startTime; t <= zone.endTime; t += zone.beatInterval)
            {
                bool isTooClose = false;
                foreach (float waveTime in waveTimes)
                {
                    if (Mathf.Abs(t - waveTime) < zone.safetyMargin)
                    {
                        isTooClose = true;
                        break;
                    }
                }

                if (isTooClose) continue;

                int safeLane = Random.Range(0, 3);
                for (int lane = 0; lane < 3; lane++)
                {
                    if (lane == safeLane) continue;

                    if (Random.value <= zone.probability)
                    {
                        GameObject cube = (GameObject)PrefabUtility.InstantiatePrefab(cubePrefab);
                        
                        float xPos = levelExporter.Player.position.x;
                        if (lane == 0) xPos -= zone.laneOffset;
                        else if (lane == 2) xPos += zone.laneOffset;

                        float zPos = t * levelExporter.Speed;
                        cube.transform.position = new Vector3(xPos, cube.transform.position.y, zPos);
                        cube.transform.SetParent(procBlocksTf);
                        Undo.RegisterCreatedObjectUndo(cube, "Spawn Procedural Cube");
                    }
                }
            }
        }
    }
}
#endif