using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] _sounds;

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

            if (s.PlayOnAwake)
            {
                s.Source.Play();
            }
        }
    }

    public void PlaySound(string name)
    {
        try
        {
            Sound s = Array.Find(_sounds, sound => sound.Name == name);
            s.Source.Play();
            s.Source.volume = s.Volume;
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
            s.Source.volume = s.Volume;
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
    {
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
    {
        foreach (Sound s in _sounds)
        {
            if (s.Source.isPlaying)
            {
                s.Source.Stop();
            }
        }
    }

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
        try
        {
            StartCoroutine(FadeVolumeCoroutine(name, duration, targetVolume));
        }
        catch
        {
            Debug.LogWarning(name + " sound not found");
        }
    }

    private IEnumerator FadeVolumeCoroutine(string name, float duration, float targetVolume)
    {
        Sound s = Array.Find(_sounds, sound => sound.Name == name);
        float startVolume = s.Source.volume;
        float timer = 0;

        while (s.Source.volume != targetVolume)
        {
            s.Source.volume = Mathf.Lerp(startVolume, targetVolume, timer/duration);
            timer += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    public float GetLength(string name)
    {
        Sound s = Array.Find(_sounds, sound => sound.Name == name);
        return s.Source.clip.length;
    }
    
    public void PauseSound(string name)
    {
        Sound s = Array.Find(_sounds, sound => sound.Name == name);
        if (s != null && s.Source.isPlaying)
        {
            s.Source.Pause();
        }
    }

    public void ResumeSound(string name)
    {
        Sound s = Array.Find(_sounds, sound => sound.Name == name);
        if (s != null)
        {
            s.Source.UnPause();
        }
    }
}
