using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private PauseMenu pauseMenu;

    private void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void ShowSettingsMenu()
    {
        if (pauseMenu != null && pauseMenu.containerPause != null)
        {
            pauseMenu.containerPause.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void HideSettingsMenu()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (pauseMenu != null && pauseMenu.containerPause != null)
        {
            pauseMenu.containerPause.SetActive(true);
        }
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
