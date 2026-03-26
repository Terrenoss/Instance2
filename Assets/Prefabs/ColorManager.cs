using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WaveMaterialConfig
{
    public WaveTypeEnum waveType;
    public Material material;
    public Material feedbackMaterial;
    public List<LineRenderer> targets;
}

public class ColorManager : MonoBehaviour
{
    public Material albedo;
    public static ColorManager Instance;
    public WaveMaterialConfig[] waveConfigs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ApplyAll()
    {
        foreach (var config in waveConfigs)
            Apply(config);
    }

    public void Apply(WaveTypeEnum waveType)
    {
        foreach (var config in waveConfigs)
        {
            if (config.waveType != waveType) continue;
            Apply(config);
            return;
        }
        Debug.LogWarning($"[ColorManager] No config found for {waveType}");
    }

    public Material GetMaterial(WaveTypeEnum waveType)
    {
        foreach (var config in waveConfigs)
            if (config.waveType == waveType)
                return config.material;
        return null;
    }

    private void Apply(WaveMaterialConfig config)
    {
        if (config.material == null) return;
        foreach (var lr in config.targets)
        {
            if (lr == null || !lr) continue;
            var osc = lr.GetComponent<OscilloscopeObjects>();
            if (osc != null)
                osc.Setmaterail(config.material);
            else
                lr.material = config.material;
        }
    }

    public void ApplyEmptyWaves(Material material)
    {
        if (material == null) return;
        {
            
        }
    }

    public Material GetFeedbackMaterial(WaveTypeEnum waveType)
    {
        foreach (var config in waveConfigs)
            if (config.waveType == waveType)
                return config.feedbackMaterial;
        return null;
    }
}