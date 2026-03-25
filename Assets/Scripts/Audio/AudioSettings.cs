using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private string _masterMixParamName;
    [SerializeField] private string _musicMixParamName;
    [SerializeField] private string _sfxMixParamName;
    
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    public event Action OnVolumeChanged;
    private SaveableDatas datas = new("AudioSettings");

    private void Start()
    {
        LoadVolumes();
        OnVolumeChanged += SaveVolumes;
    }

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
    
    private void SaveVolumes()
    {
        List<float> volumes = GetVolumes();
        datas.SaveFloat("masterVolume",volumes[0]);
        datas.SaveFloat("musicVolume",volumes[1]);
        datas.SaveFloat("sfxVolume",volumes[2]);

        SaveSystem.SaveData(datas);
    }

    private void LoadVolumes()
    {
        SaveableDatas datas = SaveSystem.LoadDatas("AudioSettings");

        float masterVolume;
        float musicVolume;
        float sfxVolume;
        
        if (datas != null)
        {
            masterVolume = datas.GetSavedFloat("masterVolume");
            musicVolume = datas.GetSavedFloat("musicVolume");
            sfxVolume = datas.GetSavedFloat("sfxVolume");

        }
        else
        {
            masterVolume = 0;
            musicVolume = 0;
            sfxVolume = 0;
        }
        
        SetMasterGroupAttenuationVolume(masterVolume);
        SetMusicGroupAttenuationVolume(musicVolume);
        SetSfxGroupAttenuationVolume(sfxVolume);
        
        if (masterSlider) masterSlider.value = masterVolume;
        if (musicSlider) musicSlider.value = musicVolume;
        if (sfxSlider) sfxSlider.value = sfxVolume;
    }
}
