#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RhythmAudioController
{
    private AudioClip audioClip;
    private float[] cachedSamples;
    private AudioClip lastProcessedClip;
    private float currentTime = 0f;

    public AudioClip Clip => audioClip;
    public float[] CachedSamples => cachedSamples;
    public AudioClip LastProcessedClip => lastProcessedClip;
    public float CurrentTime => currentTime;

    public float volume = 0.15f;
    
    public GameObject hiddenAudioPlayer;
    public AudioSource audioSource;
    public bool autoScroll = true;

    public void OnEnable()
    {
        int oldPlayerId = SessionState.GetInt("RhythmEditor_HiddenPlayerID", 0);
        if (oldPlayerId != 0)
        {
            GameObject oldPlayer = EditorUtility.InstanceIDToObject(oldPlayerId) as GameObject;
            if (oldPlayer != null) Object.DestroyImmediate(oldPlayer);
        }
        
        if (audioClip != null) CacheAudioSamples();
    }

    public void OnDisable()
    {
        if (hiddenAudioPlayer != null) Object.DestroyImmediate(hiddenAudioPlayer);
    }

    public void UpdateTimeFromSource()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            currentTime = audioSource.time;
        }
    }

    public void SetAudioTime(float time)
    {
        currentTime = time;
        if (audioSource != null) audioSource.time = currentTime;
    }

    public void SetAudioClip(AudioClip newClip)
    {
        audioClip = newClip;
        CacheAudioSamples();
        if (audioSource != null) audioSource.clip = audioClip;
    }

    public void PlayAudio()
    {
        if (audioClip == null) return;

        autoScroll = true; 

        if (hiddenAudioPlayer == null)
        {
            hiddenAudioPlayer = new GameObject("Hidden_Rhythm_AudioPlayer");
            hiddenAudioPlayer.hideFlags = HideFlags.HideAndDontSave;
            audioSource = hiddenAudioPlayer.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            
            SessionState.SetInt("RhythmEditor_HiddenPlayerID", hiddenAudioPlayer.GetInstanceID());
        }
        
        audioSource.clip = audioClip;
        audioSource.volume = Mathf.Pow(volume, 3);
        audioSource.time = currentTime;
        
        if (!audioSource.isPlaying) audioSource.Play();
    }

    public void PauseAudio()
    {
        if (audioSource != null && audioSource.isPlaying) audioSource.Pause();
    }

    public void StopAudio()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            SetAudioTime(0f);
        }
    }

    public void CacheAudioSamples()
    {
        if (audioClip != null)
        {
            lastProcessedClip = audioClip;
            cachedSamples = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(cachedSamples, 0);
        }
        else
        {
            cachedSamples = null;
        }
    }
    
    public bool IsPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }
}
#endif
