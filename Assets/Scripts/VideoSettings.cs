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
    private bool showWavesColor = true;
    private SaveableDatas datas = new("VideoSettings");

    private void Start()
    {
        LoadSettings();
        ToggleWavesColor();
        ToggleFullScreen();
        
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

    public void ToggleWavesColor()
    {
        showWavesColor = !showWavesColor;
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
            isFullScreen = !datas.GetSavedBool("isFullScreen");
            showWavesColor =  !datas.GetSavedBool("isWavesColorVisible");
        }
        else
        {
            isFullScreen = false;
            showWavesColor = false;
        }

        //actualise button
    }
}
