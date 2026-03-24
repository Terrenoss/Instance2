using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject containerPause;
    public bool isGamePaused = false;
    [SerializeField] private string mainMenu;
    [SerializeField] private Settings settingsLogic;
    [SerializeField] private AudioManager audioManager;
    public bool isVolumeZero = false;

    private void Awake()
    {
        if (settingsLogic != null)
        {
            settingsLogic.OnSettingsOpened += HandleSettingsOpened;
            settingsLogic.OnSettingsClosed += HandleSettingsClosed;
        }
    }

    private void Start()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        
        if (containerPause != null) containerPause.SetActive(false);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void TogglePause()
    {
        if (isGamePaused)
        {
            if (settingsLogic != null && settingsLogic.IsSettingsPanelActive())
            {
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

        if (!isVolumeZero)
        {
            Debug.Log("can Pause music");
            audioManager.PauseSound("MainMusic");
        }
        
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
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        
        if(!isVolumeZero)
        { 
            Debug.Log("can resume music");
            audioManager.ResumeSound("MainMusic");
        }

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
    }

    private void HandleSettingsOpened()
    {
        if (containerPause != null)
        {
            containerPause.SetActive(false);
        }
    }

    private void HandleSettingsClosed()
    {
        if (containerPause != null && isGamePaused)
        {
            containerPause.SetActive(true);
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    private void OnDestroy()
    {
        if (settingsLogic != null)
        {
            settingsLogic.OnSettingsOpened -= HandleSettingsOpened;
            settingsLogic.OnSettingsClosed -= HandleSettingsClosed;
        }
        Time.timeScale = 1;
    }
}
