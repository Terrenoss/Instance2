using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelExporter))]
public class LevelExporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelExporter exporter = (LevelExporter)target;

        if (GUILayout.Button("Export Level"))
        {
            exporter.ExportLevel();
        }
    }
}