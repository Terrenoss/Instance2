using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] _sounds;
    private float _timeBetweenMusics;

    void Awake()
    {
        foreach (Sound s in _sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.outputAudioMixerGroup = s.AudioMixerGroup;

            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
            s.Source.playOnAwake = s.PlayOnAwake;
        }
    }

    #region music
    private void Start()
    {
        PlaySound("Music");
    }
    private void Update()
    {
        _timeBetweenMusics += Time.deltaTime;
        if (_timeBetweenMusics >= GetLength("Music"))
        {
            PlaySound("Music");
        }
    }
    #endregion

    #region sfx
    public void PlaySound(string name)
    {
        try
        {
            Sound s = Array.Find(_sounds, sound => sound.Name == name);
            s.Source.Play();
        }
        catch
        {
            Debug.LogWarning(name + " sound not found");
        }
    }

    public void PlayOverlap(string name)
    {
        try
        {
            Sound s = Array.Find(_sounds, sound => sound.Name == name);
            s.Source.PlayOneShot(s.Source.clip, s.Source.volume);
        }
        catch
        {
            Debug.LogWarning(name + " sound not found");
        }
    }

    public void PlayDelay(string name, float delay)
    {
        StartCoroutine(PlayDelayCoroutine(delay, name));
    }

    private IEnumerator PlayDelayCoroutine(float delay, string name)
    {
        yield return new WaitForSeconds(delay);
        PlayOverlap(name);
    }

    public void StopSound(string name)
    { //pas utilisé
        try
        {
            Sound s = Array.Find(_sounds, sound => sound.Name == name);
            s.Source.Stop();
        }
        catch
        {
            Debug.LogWarning(name + " sound not found");
        }
    }

    public void stopAllSounds()
    { //pas utilisé
        foreach (Sound s in _sounds)
        {
            if (s.Source.isPlaying)
            {
                s.Source.Stop();
            }
        }
    }
    #endregion

    #region settings
    public void ChangeVolume(string name, float volume)
    {
        Sound s = Array.Find(_sounds, sound => sound.Name == name);
        s.Source.volume = volume;
    }

    public void ChangePitch(string name, float pitch)
    {
        Sound s = Array.Find(_sounds, sound => sound.Name == name);
        s.Source.pitch = pitch;
    }

    public void FadeVolume(string name, float duration, float targetVolume)
    {
        StartCoroutine(FadeVolumeCoroutine(name, duration, targetVolume));
    }

    private IEnumerator FadeVolumeCoroutine(string name, float duration, float targetVolume)
    {
        Sound s = Array.Find(_sounds, sound => sound.Name == name);
        float startVolume = s.Source.volume;

        while (s.Source.volume > 0)
        {
            s.Source.volume -= startVolume * Time.deltaTime / duration;

            yield return null;
        }

        s.Source.Stop();
        s.Source.volume = startVolume;
        yield return null;
    }
    #endregion

    #region parameters
    public float GetLength(string name)
    {
        Sound s = Array.Find(_sounds, sound => sound.Name == name);
        return s.Source.clip.length;
    }
    #endregion


    //placer dans nimporte quel scrypt avec le bon nom dans les "" pour jouer un son
    //FindObjectOfType<AudioManager>().X("");
    //X is the name of the function called
}
