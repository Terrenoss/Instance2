using System;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    public event Action OnSettingsOpened;
    public event Action OnSettingsClosed;

    private void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void ShowSettingsMenu()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }

        OnSettingsOpened?.Invoke();
    }

    public void HideSettingsMenu()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        OnSettingsClosed?.Invoke();
    }

    public void ToggleSettingsMenu()
    {
        if (settingsPanel != null)
        {
            if (settingsPanel.activeSelf)
            {
                HideSettingsMenu();
            }
            else
            {
                ShowSettingsMenu();
            }
        }
    }

    public bool IsSettingsPanelActive()
    {
        return settingsPanel != null && settingsPanel.activeSelf;
    }
}
