using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.UI;

public class VideoSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    Resolution[] _resolutions;
    [SerializeField] private Sprite SpriteOn;
    [SerializeField] private Sprite SpriteOff;
    [SerializeField] private Button fullScreenButton;
    [SerializeField] private TextMeshProUGUI fullScreenText;
    [SerializeField] private Button showWaveColorButton;
    [SerializeField] private TextMeshProUGUI showWaveColorText;
    private bool isFullScreen = true;
    public bool showWavesColor = true;
    public event Action OnWavesColorChanged;
    private SaveableDatas datas = new("VideoSettings");

    private void Awake()
    {
        LoadSettings();
        ApplySettings();
        
        _resolutions = Screen.resolutions;

        List<string> _resolutionsStringList = new List<string>();
        int _currentResolutionInd = 0;
        int increment = 0;

        foreach (Resolution res in _resolutions)
        {
            _resolutionsStringList.Add(res.ToString());

            if (res.ToString() == Screen.currentResolution.ToString())
            {
                _currentResolutionInd = increment;
            }
            increment++;
        }

        _dropdown.AddOptions(_resolutionsStringList);

        _dropdown.value = _currentResolutionInd;
    }

    public void ChangeResolution()
    {
        Screen.SetResolution(_resolutions[_dropdown.value].width, _resolutions[_dropdown.value].height, Screen.fullScreen);
    }

    private void ApplySettings()
    {
        showWaveColorText.text = showWavesColor ? "on" : "off";
        showWaveColorButton.image.sprite = showWavesColor ? SpriteOn : SpriteOff;
        OnWavesColorChanged?.Invoke();

        fullScreenText.text = isFullScreen ? "on" : "off";
        fullScreenButton.image.sprite = isFullScreen ? SpriteOn : SpriteOff;
        Screen.fullScreen = isFullScreen;
    }
    
    public void ToggleWavesColor()
    {
        showWavesColor = !showWavesColor;
        OnWavesColorChanged?.Invoke();
        showWaveColorText.text = showWavesColor ? "on" : "off";
        showWaveColorButton.image.sprite = showWavesColor ? SpriteOn : SpriteOff;
        SaveSettings();
        
    }

    public void ToggleFullScreen()
    {
        isFullScreen = !isFullScreen;
        fullScreenText.text = isFullScreen ? "on" : "off";
        fullScreenButton.image.sprite = isFullScreen ? SpriteOn : SpriteOff;
        Screen.fullScreen = isFullScreen;

        SaveSettings();
    }

    private void SaveSettings()
    {
        datas.SaveBool("isFullScreen" ,isFullScreen);
        datas.SaveBool("isWavesColorVisible" ,showWavesColor);
        
        SaveSystem.SaveData(datas);
    }

    private void LoadSettings()
    {
        SaveableDatas datas = SaveSystem.LoadDatas("VideoSettings");

        if (datas != null)
        {
            isFullScreen = datas.GetSavedBool("isFullScreen");
            showWavesColor =  datas.GetSavedBool("isWavesColorVisible");
        }
        else
        {
            isFullScreen = true;
            showWavesColor = true;
        }
    }
}
