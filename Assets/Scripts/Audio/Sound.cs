using UnityEngine.Audio;
using UnityEngine;
[System.Serializable]
public class Sound
{
    public string Name;

    public AudioClip Clip;
    public AudioMixerGroup AudioMixerGroup; //can be null

    [Range(0f, 1f)]
    public float Volume;
    [Range(.1f, 3f)]
    public float Pitch; //set to 1 for normal sound
    public bool Loop;
    public bool PlayOnAwake;

    [HideInInspector]
    public AudioSource Source;
}
