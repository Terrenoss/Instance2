using UnityEngine;

public class VideoSettings : MonoBehaviour
{
    public void ToggleFullScreen(bool value)
    {
        Screen.fullScreen = value;
    }
}
