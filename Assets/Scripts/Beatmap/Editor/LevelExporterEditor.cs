using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(LevelExporter))]
public class LevelExporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelExporter exporter = (LevelExporter)target;

        GUIStyle bigButtonStyle = new GUIStyle(GUI.skin.button) { fixedHeight = 40, fontStyle = FontStyle.Bold };
        if (GUILayout.Button("Open Rhythm Editor", bigButtonStyle))
        {
            RhythmEditorWindow.ShowWindowWithExporter((LevelExporter)target);
        }
    }
}
#endif
