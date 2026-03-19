using UnityEngine;

public enum ObstacleType
{
    cube,
    wave
}

public enum WaveTypeSelection
{
    Wave1,
    Wave2,
    Wave3,
    Wave4
}

public class LevelObject : MonoBehaviour
{
    public ObstacleType type;
    public WaveTypeSelection waveType;

    private void OnDrawGizmos()
    {
        if (type == ObstacleType.wave)
        {
#if UNITY_EDITOR
            if (UnityEditor.Selection.activeGameObject == gameObject)
            {
                Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
            }
            else
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            }
#else
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
#endif
            float marginZ = 5f;
            Gizmos.DrawCube(transform.position, new Vector3(3f, 1f, marginZ));
        }
    }
}