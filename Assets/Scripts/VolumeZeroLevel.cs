using System;
using System.Collections.Generic;
using UnityEngine;

public class VolumeZeroLevel : MonoBehaviour
{
    [SerializeField] private AudioSettings audioSettings;
    [SerializeField] private SplineMover[] splineMovers;
    [SerializeField] private Spawner spawner;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private PauseMenu pauseMenu;
    private float masterVolume;
    private float musicVolume;
    private float sfxVolume;
    private float minimumSliderValue = -80f;

    private void Start()
    {
        if (audioSettings != null)
        {
            audioSettings.OnVolumeChanged += VolumeCheck;
        }
        
        VolumeCheck();
    }

    private void VolumeCheck()
    {
        List<float> volumes = audioSettings.GetVolumes();
        masterVolume = volumes[0];
        musicVolume = volumes[1];
        sfxVolume = volumes[2];

        if (masterVolume <= minimumSliderValue || (musicVolume <= minimumSliderValue && sfxVolume <= minimumSliderValue))
        {
            LevelUpdate(false);
        }
        else
        {
            LevelUpdate(true);
        }
    }

    private void LevelUpdate(bool isActive)
    {
        pauseMenu.isVolumeZero = !isActive;
        
        if (isActive)
        {
            audioManager.PauseSound("MainMusic");
        }
        else
        {
            pauseMenu.isVolumeZero = true;
        }
        
        foreach (SplineMover mover in splineMovers)
        {
            if (mover == null) continue;
                
            WaveUpdate(mover, isActive);
        }
        
        spawner.enabled = isActive;

        if (scoreManager == null) return;
        
        scoreManager.SetCanGainScore(isActive);
    }
    
    private void WaveUpdate(SplineMover mover, bool isActive)
    {
        BoxCollider collider = mover.GetComponent<BoxCollider>();
        if (collider) collider.enabled = isActive;

        Renderer rend = mover.GetComponent<Renderer>();
        if (rend) rend.enabled = isActive;

        mover.enabled = isActive;
    }
}
