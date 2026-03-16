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
}