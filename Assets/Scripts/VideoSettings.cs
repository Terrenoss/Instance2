using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;

public class VideoSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    Resolution[] _resolutions;

    private void Start()
    {
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

    public void ToggleFullScreen(bool value)
    {
        Screen.fullScreen = value;
    }
}
