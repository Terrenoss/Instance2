#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class RhythmInspectorUI
{
    public static void DrawHeader(IRhythmEditorContext context)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Rhythm Editor", EditorStyles.boldLabel);
        
        bool showHelp = context.ShowHelp;
        if (GUILayout.Button("❔ Aide / Raccourcis", GUILayout.Width(140)))
        {
            showHelp = !showHelp;
            context.ShowHelp = showHelp;
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

    public static void DrawSettings(IRhythmEditorContext context)
    {
        GUILayout.Label("1. Settings", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        AudioClip newClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", context.AudioController.Clip, typeof(AudioClip), false);
        if (EditorGUI.EndChangeCheck())
        {
            context.AudioController.SetAudioTime(0f);
            context.AudioController.SetAudioClip(newClip);
        }

        context.WavePrefab = (GameObject)EditorGUILayout.ObjectField("Wave Prefab", context.WavePrefab, typeof(GameObject), false);
        GUILayout.Space(15);
    }

    public static void DrawAudioPlayer(IRhythmEditorContext context)
    {
        GUILayout.Label("2. Audio Player", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        float newVol = EditorGUILayout.Slider("Volume", context.AudioController.volume, 0f, 1f);
        if (EditorGUI.EndChangeCheck())
        {
            context.AudioController.volume = newVol;
            if (context.AudioController.audioSource != null)
            {
                context.AudioController.audioSource.volume = Mathf.Pow(newVol, 3);
            }
        }

        GUILayout.BeginHorizontal();
        if (context.AudioController.IsPlaying())
        {
            if (GUILayout.Button("⏸ Pause", GUILayout.Height(30))) context.AudioController.PauseAudio();
        }
        else
        {
            if (GUILayout.Button("▶ Play", GUILayout.Height(30))) context.AudioController.PlayAudio();
        }

        GUI.backgroundColor = context.IsRecording ? new Color(1f, 0.3f, 0.3f) : Color.white;
        if (GUILayout.Button(context.IsRecording ? "🔴 RECORDING ACTIVE" : "⚪ Record Mode", GUILayout.Height(30)))
        {
            context.IsRecording = !context.IsRecording;
        }
        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("⏹ Stop", GUILayout.Height(30))) context.AudioController.StopAudio();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
    }

    public static void DrawWaveInspector(IRhythmEditorContext context)
    {
        if (context.SelectedWaves.Count > 0)
        {
            GUILayout.Label($"Selected Waves Properties ({context.SelectedWaves.Count} elements)", EditorStyles.boldLabel);
            
            WaveTypeSelection currentWaveType = context.SelectedWaves[0].waveType;
            bool currentParryVisible = context.SelectedWaves[0].isParryKeyVisible;

            EditorGUI.BeginChangeCheck();
            WaveTypeSelection newWaveType = (WaveTypeSelection)EditorGUILayout.EnumPopup("Wave Type", currentWaveType);
            bool newParryVisible = EditorGUILayout.Toggle("Is Parry Key Visible", currentParryVisible);
            
            if (EditorGUI.EndChangeCheck())
            {
                foreach (LevelObject wave in context.SelectedWaves)
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
                foreach (LevelObject wave in context.SelectedWaves)
                {
                    if (wave != null && wave.gameObject != null)
                        Undo.DestroyObjectImmediate(wave.gameObject);
                }
                context.SelectedWaves.Clear();
                GUI.backgroundColor = Color.white;
                GUIUtility.ExitGUI();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.Space(20);
        }
    }

    public static void DrawZoneInspector(IRhythmEditorContext context)
    {
        if (context.SelectedZones.Count > 1) 
        {
            GUILayout.Label($"Selected Zones Properties ({context.SelectedZones.Count} elements)", EditorStyles.boldLabel);
            
            float commonProb = context.SelectedZones[0].probability;
            float commonBeat = context.SelectedZones[0].beatInterval;
            float commonLane = context.SelectedZones[0].laneOffset;
            float commonSafety = context.SelectedZones[0].safetyMargin;
            Color commonCol = context.SelectedZones[0].zoneColor;

            EditorGUI.BeginChangeCheck();
            float newProb = EditorGUILayout.Slider("Spawn Density", commonProb, 0f, 1f);
            Color newCol = EditorGUILayout.ColorField("Color", commonCol);
            float newBeat = EditorGUILayout.FloatField("Beat (s)", commonBeat);
            float newLane = EditorGUILayout.FloatField("Lane Offset", commonLane);
            float newSafety = EditorGUILayout.FloatField("Safety (s)", commonSafety);
            
            if (EditorGUI.EndChangeCheck())
            {
                foreach (FrequencyZone z in context.SelectedZones)
                {
                    z.probability = newProb;
                    z.zoneColor = newCol;
                    z.beatInterval = newBeat;
                    z.laneOffset = newLane;
                    z.safetyMargin = newSafety;
                }
                if (context.LevelExporter != null) EditorUtility.SetDirty(context.LevelExporter);
            }
            GUILayout.Space(20);
        }
    }

    public static void DrawStatsAndCleanup(IRhythmEditorContext context)
    {
        GUILayout.Label("Stats & Clean Up", EditorStyles.boldLabel);
        
        int waveCount = 0;
        foreach (LevelObject obj in context.CachedLevelObjects)
            if (obj != null && obj.type == ObstacleType.wave) waveCount++;

        int procBlocksCount = 0;
        Transform procBlocksTf = context.LevelExporter != null && context.LevelExporter.BlocksParent != null ? context.LevelExporter.BlocksParent.Find("ProceduralBlocks") : null;
        if (procBlocksTf != null)
            procBlocksCount = procBlocksTf.childCount;

        GUILayout.Label($"Waves count: {waveCount}");
        GUILayout.Label($"Procedural Blocks count: {procBlocksCount}");

        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear All Waves", GUILayout.Height(30)))
        {
            foreach (LevelObject obj in context.CachedLevelObjects)
                if (obj != null && obj.type == ObstacleType.wave) Undo.DestroyObjectImmediate(obj.gameObject);
            context.SelectedWaves.Clear();
        }
        if (GUILayout.Button("Clear Procedural Blocks", GUILayout.Height(30)))
        {
            if (procBlocksTf != null) Undo.DestroyObjectImmediate(procBlocksTf.gameObject);
        }
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
    }

    public static void DrawProceduralSection(IRhythmEditorContext context)
    {
        GUILayout.Label("Procedural Generation", EditorStyles.boldLabel);
        context.CubePrefab = (GameObject)EditorGUILayout.ObjectField("Cube Prefab", context.CubePrefab, typeof(GameObject), false);
        
        GUILayout.Space(5);
        GUILayout.Label("Frequency Zones:", EditorStyles.boldLabel);
        
        for (int i = 0; i < context.Zones.Count; i++)
        {
            if (DrawZoneElement(context, context.Zones[i], i)) i--;
        }
        
        if (GUILayout.Button("Add Zone")) 
        {
            context.Zones.Add(new FrequencyZone());
            if (context.LevelExporter != null) EditorUtility.SetDirty(context.LevelExporter);
        }

        GUILayout.Space(10);
        GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
        if (GUILayout.Button("Generate Procedural Blocks", GUILayout.Height(40)))
        {
            RhythmProceduralGenerator.GenerateProceduralBlocks(context.LevelExporter, context.CubePrefab, context.AudioController.Clip, context.Zones, context.CachedLevelObjects);
        }
        GUI.backgroundColor = Color.white;
        GUILayout.Space(20);
    }

    private static bool DrawZoneElement(IRhythmEditorContext context, FrequencyZone zone, int index)
    {
        if (context.SelectedZones.Contains(zone)) 
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
            FrequencyZone temp = context.Zones[index]; context.Zones[index] = context.Zones[index - 1]; context.Zones[index - 1] = temp;
            if (context.LevelExporter != null) EditorUtility.SetDirty(context.LevelExporter);
        }
        if (GUILayout.Button("▼", GUILayout.Width(25)) && index < context.Zones.Count - 1)
        {
            FrequencyZone temp = context.Zones[index]; context.Zones[index] = context.Zones[index + 1]; context.Zones[index + 1] = temp;
            if (context.LevelExporter != null) EditorUtility.SetDirty(context.LevelExporter);
        }
        if (GUILayout.Button("X", GUILayout.Width(25)))
        {
            if (context.SelectedZones.Contains(zone)) context.SelectedZones.Remove(zone);
            context.Zones.RemoveAt(index);
            if (context.LevelExporter != null) EditorUtility.SetDirty(context.LevelExporter);
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

    public static void DrawActions(IRhythmEditorContext context)
    {
        GUILayout.Label("4. Actions", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Export Level to JSON", GUILayout.Height(40)))
        {
            if (context.LevelExporter != null) 
            {
                UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
                context.LevelExporter.ExportLevel();
            }
            else Debug.LogWarning("Veuillez assigner le Level Exporter !");
        }
        GUI.backgroundColor = Color.white;
        GUILayout.Space(60); 
    }
}
#endif
