using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject containerPause;
    public bool isGamePaused = false;
    [SerializeField] private string mainMenu;
    [SerializeField] private Settings settingsLogic;

    private void Start()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        
        if (containerPause != null) containerPause.SetActive(false);
    }

    public void TogglePause()
    {
        Debug.Log(">>> PauseMenu.TogglePause() was called! isGamePaused currently = " + isGamePaused);
        if (isGamePaused)
        {
            if (settingsLogic != null && settingsLogic.IsSettingsPanelActive())
            {
                Debug.Log(">>> Closing settings panel and reopening pause menu.");
                settingsLogic.HideSettingsMenu();
            }
            else
            {
                ResumeGame();
            }
        }
        else
        {
            PausedGame();
        }
    }

    public void PausedGame()
    {
        isGamePaused = true;
        Time.timeScale = 0;
        
        if (settingsLogic != null && settingsLogic.IsSettingsPanelActive())
        {
            settingsLogic.HideSettingsMenu();
        }

        if (containerPause != null)
        {
            containerPause.SetActive(true);
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Paused Game " + isGamePaused);
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        
        if (settingsLogic != null && settingsLogic.IsSettingsPanelActive())
        {
            settingsLogic.HideSettingsMenu();
        }

        if (containerPause != null)
        {
            containerPause.SetActive(false);
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("Paused Game " + isGamePaused);
    }

    public void ShowSettingMenu()
    {
        if (settingsLogic != null)
        {
            settingsLogic.ShowSettingsMenu();
        }
    }

    public void ReturnToMainMenu()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(mainMenu);
        Debug.Log("Paused Game " + isGamePaused);
    }

    private void OnDisable()
    {
        // Forcer la remise à la normale du temps si le menu est désactivé
        Time.timeScale = 1;
    }

    private void OnDestroy()
    {
        // Sécurité ultime lors du déchargement de la scène
        Time.timeScale = 1;
    }
}
