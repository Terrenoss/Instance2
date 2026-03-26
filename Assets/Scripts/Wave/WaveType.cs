using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public enum WaveTypeEnum
{
    wave1,
    wave2,
    wave3,
    wave4,
}

public class WaveData
{
    public Color color;
    public Guid keyId;
    public string keyName;
}

public class WaveType : MonoBehaviour
{    
    public WaveTypeEnum waveTypeEnum;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject waveParryUI;
    [SerializeField] private TextMeshProUGUI waveParryKey;
    [SerializeField] private VideoSettings videoSettings;
    private string parryKeyString;
    private Dictionary<string, string> _specialCharacters = new Dictionary<string, string>();
    private List<Guid> bindingList = new();
    private List<string> bindingListName = new();
    private Renderer rend;
    private bool isColorVisible = true;
    [SerializeField] private WaveMaterialConfig[] waveConfigs;
    [SerializeField] private Material material; 
    private Material runtimeMaterial;

    public Dictionary<WaveTypeEnum, WaveData> waveParam = new()
    {
        { WaveTypeEnum.wave1, new WaveData() {color = Color.red} },
        { WaveTypeEnum.wave2, new WaveData() {color = Color.green} },
        { WaveTypeEnum.wave3, new WaveData() {color = Color.blue} },
        { WaveTypeEnum.wave4, new WaveData() {color = Color.yellow} }
    };
    
    private WaveMaterialConfig? GetConfig()
    {
        foreach (var config in waveConfigs)
        {
            if (config.waveType == waveTypeEnum)
                return config;
        }
        return null;
    }
    
    private void ApplyLocalColor()
    {
        var config = GetConfig();
        if (config == null) return;

        if (rend == null)
            rend = GetComponent<Renderer>();

        if (runtimeMaterial == null)
            runtimeMaterial = new Material(config.Value.material);

        if (rend != null)
            rend.material = runtimeMaterial;

        foreach (var lr in config.Value.targets)
        {
            if (lr == null) continue;

            var osc = lr.GetComponent<OscilloscopeObjects>();
            if (osc != null)
                osc.Setmaterail(runtimeMaterial);
            else
                lr.material = runtimeMaterial;
        }
    }

    private void clsColor()
    {
        if (rend == null)
            rend = GetComponent<Renderer>();

        if (runtimeMaterial == null)
            runtimeMaterial = new Material(material);

        if (rend != null)
            rend.material = runtimeMaterial;

        var config = GetConfig();
        if (config == null) return;

        foreach (var lr in config.Value.targets)
        {
            if (lr == null) continue;

            var osc = lr.GetComponent<OscilloscopeObjects>();
            if (osc != null)
                osc.Setmaterail(runtimeMaterial);
            else
                lr.material = runtimeMaterial;
        }
    }

    private void Awake()
    {
        _specialCharacters.Add("Up Arrow", "\u2191");
        _specialCharacters.Add("Down Arrow", "\u2193");
        _specialCharacters.Add("Left Arrow", "\u2190");
        _specialCharacters.Add("Right Arrow", "\u2192");
        
        InputAction action = playerInput.actions["Parry"];
        ReadOnlyArray<InputBinding> bindings = action.bindings;

        foreach (InputBinding binding in bindings)
        {
            Guid key = binding.id;
            string keyName = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            bindingListName.Add(keyName);
            bindingList.Add(key);
        }

        waveParam[WaveTypeEnum.wave1].keyId = bindingList[0];
        waveParam[WaveTypeEnum.wave2].keyId = bindingList[1];
        waveParam[WaveTypeEnum.wave3].keyId = bindingList[2];
        waveParam[WaveTypeEnum.wave4].keyId = bindingList[3];        
        
        waveParam[WaveTypeEnum.wave1].keyName = bindingListName[0];
        waveParam[WaveTypeEnum.wave2].keyName = bindingListName[1];
        waveParam[WaveTypeEnum.wave3].keyName = bindingListName[2];
        waveParam[WaveTypeEnum.wave4].keyName = bindingListName[3];
        
        videoSettings.OnWavesColorChanged += UpdateColor;
        isColorVisible = videoSettings.showWavesColor;
    }

    public void SetWaveType(WaveTypeSelection waveType)
    {
        waveTypeEnum = (WaveTypeEnum)waveType;

        if (rend == null)
            rend = GetComponent<Renderer>();

        runtimeMaterial = null;

        if (isColorVisible)
            ApplyLocalColor();
        else
            clsColor();
    }

    public void SetParryKeyVisible(bool isParryKeyVisible)
    {
        waveParryUI.SetActive(isParryKeyVisible);

        if (!isParryKeyVisible) return;
        
        parryKeyString = waveParam[waveTypeEnum].keyName;
        if (_specialCharacters.ContainsKey(parryKeyString))
            parryKeyString = _specialCharacters[parryKeyString];
        
        waveParryKey.text = parryKeyString;
    }

    private void UpdateColor()
    {
        if (rend == null)
            rend = GetComponent<Renderer>();
        
        isColorVisible = videoSettings.showWavesColor;
        runtimeMaterial = null;

        if (isColorVisible)
            ApplyLocalColor();
        else
            clsColor();
    }
}