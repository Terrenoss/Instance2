using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private string _masterMixParamName;
    [SerializeField] private string _musicMixParamName;
    [SerializeField] private string _sfxMixParamName;

    public void SetMasterGroupAttenuationVolume(float value)
    {
        _audioMixer.SetFloat(_masterMixParamName, value);
    }

    public void SetMusicGroupAttenuationVolume(float value)
    {
        _audioMixer.SetFloat(_musicMixParamName, value);
    }

    public void SetSfxGroupAttenuationVolume(float value)
    {
        _audioMixer.SetFloat(_sfxMixParamName, value);
    }
}
