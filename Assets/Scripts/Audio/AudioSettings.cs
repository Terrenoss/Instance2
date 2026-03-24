using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private string _masterMixParamName;
    [SerializeField] private string _musicMixParamName;
    [SerializeField] private string _sfxMixParamName;
    public event Action OnVolumeChanged;

    public void SetMasterGroupAttenuationVolume(float value)
    {
        _audioMixer.SetFloat(_masterMixParamName, value);
        OnVolumeChanged?.Invoke();
    }

    public void SetMusicGroupAttenuationVolume(float value)
    {
        _audioMixer.SetFloat(_musicMixParamName, value);
        OnVolumeChanged?.Invoke();
    }

    public void SetSfxGroupAttenuationVolume(float value)
    {
        _audioMixer.SetFloat(_sfxMixParamName, value);
        OnVolumeChanged?.Invoke();
    }
    
    public List<float> GetVolumes()
    {
        List<float> volumes = new List<float>();

        if (!_audioMixer.GetFloat(_masterMixParamName, out float masterVol))
        {
            masterVol = -80f;
        }
        if (!_audioMixer.GetFloat(_musicMixParamName, out float musicVol))
        {
            musicVol = -80f;
        }
        if (!_audioMixer.GetFloat(_sfxMixParamName, out float sfxVol))
        {
            sfxVol = -80f;
        }

        volumes.Add(masterVol);
        volumes.Add(musicVol);
        volumes.Add(sfxVol);

        return volumes;
    }
}
