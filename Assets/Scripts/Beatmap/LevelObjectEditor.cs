using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelObject))]
public class LevelObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelObject obj = (LevelObject)target;

        obj.type = (ObstacleType)EditorGUILayout.EnumPopup("Type", obj.type);

        if (obj.type == ObstacleType.wave)
        {
            obj.waveType = (WaveTypeSelection)EditorGUILayout.EnumPopup("Wave Type", obj.waveType);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(obj);
        }
    }
}