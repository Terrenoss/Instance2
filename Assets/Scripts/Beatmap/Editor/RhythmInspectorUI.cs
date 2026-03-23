#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class RhythmInspectorUI
{
    public static void DrawHeader(ref bool showHelp)
    {
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
                "• [Maj + Flèches] : Déplacer les éléments 5x plus vite\n" +
                "• [Zones] : Attrapez le bord pour redimensionner, et le centre pour la déplacer.", MessageType.Info);
            GUILayout.Space(10);
        }
        GUILayout.Space(10);
    }

    public static void DrawSettings(RhythmEditorWindow window)
    {
        GUILayout.Label("1. Settings", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        AudioClip newClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", window.audioController.Clip, typeof(AudioClip), false);
        if (EditorGUI.EndChangeCheck())
        {
            window.audioController.SetAudioTime(0f);
            window.audioController.SetAudioClip(newClip);
        }

        window.wavePrefab = (GameObject)EditorGUILayout.ObjectField("Wave Prefab", window.wavePrefab, typeof(GameObject), false);
        GUILayout.Space(15);
    }

    public static void DrawAudioPlayer(RhythmEditorWindow window)
    {
        GUILayout.Label("2. Audio Player", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        float newVol = EditorGUILayout.Slider("Volume", window.audioController.volume, 0f, 1f);
        if (EditorGUI.EndChangeCheck())
        {
            window.audioController.volume = newVol;
            if (window.audioController.audioSource != null)
            {
                window.audioController.audioSource.volume = Mathf.Pow(newVol, 3);
            }
        }

        GUILayout.BeginHorizontal();
        if (window.audioController.IsPlaying())
        {
            if (GUILayout.Button("⏸ Pause", GUILayout.Height(30))) window.audioController.PauseAudio();
        }
        else
        {
            if (GUILayout.Button("▶ Play", GUILayout.Height(30))) window.audioController.PlayAudio();
        }

        GUI.backgroundColor = window.isRecording ? new Color(1f, 0.3f, 0.3f) : Color.white;
        if (GUILayout.Button(window.isRecording ? "🔴 RECORDING ACTIVE" : "⚪ Record Mode", GUILayout.Height(30)))
        {
            window.isRecording = !window.isRecording;
        }
        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("⏹ Stop", GUILayout.Height(30))) window.audioController.StopAudio();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
    }

    public static void DrawWaveInspector(RhythmEditorWindow window)
    {
        if (window.selectedWaves.Count > 0)
        {
            GUILayout.Label($"Selected Waves Properties ({window.selectedWaves.Count} elements)", EditorStyles.boldLabel);
            
            WaveTypeSelection currentWaveType = window.selectedWaves[0].waveType;
            bool currentParryVisible = window.selectedWaves[0].isParryKeyVisible;

            EditorGUI.BeginChangeCheck();
            WaveTypeSelection newWaveType = (WaveTypeSelection)EditorGUILayout.EnumPopup("Wave Type", currentWaveType);
            bool newParryVisible = EditorGUILayout.Toggle("Is Parry Key Visible", currentParryVisible);
            
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var wave in window.selectedWaves)
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
                foreach (var wave in window.selectedWaves)
                {
                    if (wave != null && wave.gameObject != null)
                        Undo.DestroyObjectImmediate(wave.gameObject);
                }
                window.selectedWaves.Clear();
                GUI.backgroundColor = Color.white;
                GUIUtility.ExitGUI();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.Space(20);
        }
    }

    public static void DrawZoneInspector(RhythmEditorWindow window)
    {
        if (window.selectedZones.Count > 1) 
        {
            GUILayout.Label($"Selected Zones Properties ({window.selectedZones.Count} elements)", EditorStyles.boldLabel);
            
            float commonProb = window.selectedZones[0].probability;
            float commonBeat = window.selectedZones[0].beatInterval;
            float commonLane = window.selectedZones[0].laneOffset;
            float commonSafety = window.selectedZones[0].safetyMargin;
            Color commonCol = window.selectedZones[0].zoneColor;

            EditorGUI.BeginChangeCheck();
            float newProb = EditorGUILayout.Slider("Spawn Density", commonProb, 0f, 1f);
            Color newCol = EditorGUILayout.ColorField("Color", commonCol);
            float newBeat = EditorGUILayout.FloatField("Beat (s)", commonBeat);
            float newLane = EditorGUILayout.FloatField("Lane Offset", commonLane);
            float newSafety = EditorGUILayout.FloatField("Safety (s)", commonSafety);
            
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var z in window.selectedZones)
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
    }

    public static void DrawStatsAndCleanup(RhythmEditorWindow window)
    {
        GUILayout.Label("Stats & Clean Up", EditorStyles.boldLabel);
        
        int waveCount = 0;
        foreach (var obj in window.cachedLevelObjects)
            if (obj != null && obj.type == ObstacleType.wave) waveCount++;

        int procBlocksCount = 0;
        Transform procBlocksTf = window.levelExporter != null && window.levelExporter.BlocksParent != null ? window.levelExporter.BlocksParent.Find("ProceduralBlocks") : null;
        if (procBlocksTf != null)
            procBlocksCount = procBlocksTf.childCount;

        GUILayout.Label($"Waves count: {waveCount}");
        GUILayout.Label($"Procedural Blocks count: {procBlocksCount}");

        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear All Waves", GUILayout.Height(30)))
        {
            foreach (var obj in window.cachedLevelObjects)
                if (obj != null && obj.type == ObstacleType.wave) Undo.DestroyObjectImmediate(obj.gameObject);
            window.selectedWaves.Clear();
        }
        if (GUILayout.Button("Clear Procedural Blocks", GUILayout.Height(30)))
        {
            if (procBlocksTf != null) Undo.DestroyObjectImmediate(procBlocksTf.gameObject);
        }
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
    }

    public static void DrawProceduralSection(RhythmEditorWindow window)
    {
        GUILayout.Label("Procedural Generation", EditorStyles.boldLabel);
        window.cubePrefab = (GameObject)EditorGUILayout.ObjectField("Cube Prefab", window.cubePrefab, typeof(GameObject), false);
        
        GUILayout.Space(5);
        GUILayout.Label("Frequency Zones:", EditorStyles.boldLabel);
        
        for (int i = 0; i < window.zones.Count; i++)
        {
            if (DrawZoneElement(window, window.zones[i], i)) i--;
        }
        
        if (GUILayout.Button("Add Zone")) window.zones.Add(new FrequencyZone());

        GUILayout.Space(10);
        GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
        if (GUILayout.Button("Generate Procedural Blocks", GUILayout.Height(40)))
        {
            RhythmProceduralGenerator.GenerateProceduralBlocks(window.levelExporter, window.cubePrefab, window.audioController.Clip, window.zones, window.cachedLevelObjects);
        }
        GUI.backgroundColor = Color.white;
        GUILayout.Space(20);
    }

    private static bool DrawZoneElement(RhythmEditorWindow window, FrequencyZone zone, int index)
    {
        if (window.selectedZones.Contains(zone)) 
            GUI.backgroundColor = new Color(1f, 1f, 0f, 0.5f);
        
        GUILayout.BeginVertical("box");
        GUI.backgroundColor = Color.white;
        
        GUILayout.BeginHorizontal();
        GUILayout.Label($"Zone {index + 1}", EditorStyles.boldLabel, GUILayout.Width(60));
        zone.startTime = EditorGUILayout.FloatField(zone.startTime, GUILayout.Width(50));
        GUILayout.Label("-", GUILayout.Width(10));
        zone.endTime = EditorGUILayout.FloatField(zone.endTime, GUILayout.Width(50));
        
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("▲", GUILayout.Width(25)) && index > 0)
        {
            var temp = window.zones[index]; window.zones[index] = window.zones[index - 1]; window.zones[index - 1] = temp;
        }
        if (GUILayout.Button("▼", GUILayout.Width(25)) && index < window.zones.Count - 1)
        {
            var temp = window.zones[index]; window.zones[index] = window.zones[index + 1]; window.zones[index + 1] = temp;
        }
        if (GUILayout.Button("X", GUILayout.Width(25)))
        {
            if (window.selectedZones.Contains(zone)) window.selectedZones.Remove(zone);
            window.zones.RemoveAt(index);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            return true;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        zone.probability = EditorGUILayout.Slider("Spawn Density", zone.probability, 0f, 1f);
        zone.zoneColor = EditorGUILayout.ColorField(zone.zoneColor, GUILayout.Width(60));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        zone.beatInterval = EditorGUILayout.FloatField("Beat (s)", zone.beatInterval);
        zone.laneOffset = EditorGUILayout.FloatField("Lane Offset", zone.laneOffset);
        zone.safetyMargin = EditorGUILayout.FloatField("Safety (s)", zone.safetyMargin);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        return false;
    }

    public static void DrawActions(RhythmEditorWindow window)
    {
        GUILayout.Label("4. Actions", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Export Level to JSON", GUILayout.Height(40)))
        {
            if (window.levelExporter != null) window.levelExporter.ExportLevel();
            else Debug.LogWarning("Veuillez assigner le Level Exporter !");
        }
        GUI.backgroundColor = Color.white;
        GUILayout.Space(60); 
    }
}
#endif
