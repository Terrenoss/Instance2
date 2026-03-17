using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject containerPause;
    public bool isGamePaused = false;
    [SerializeField] private string mainMenu;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        
        if (containerPause != null)
        {
            containerPause.SetActive(false);
        }
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void TogglePause()
    {
        Debug.Log(">>> PauseMenu.TogglePause() was called! isGamePaused currently = " + isGamePaused);
        if (isGamePaused)
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                Debug.Log(">>> Closing settings panel and reopening pause menu.");
                settingsPanel.SetActive(false);
                if (containerPause != null)
                {
                    containerPause.SetActive(true);
                }
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
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        Time.timeScale = 0;
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
        if (containerPause != null)
        {
            containerPause.SetActive(false);
        }
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
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
}
