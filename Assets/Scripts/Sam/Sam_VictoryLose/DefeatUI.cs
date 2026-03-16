using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatUI : MonoBehaviour
{
    public GameObject containerDefeat;
    
    [SerializeField] private float timeBeforeShowDefeatMenu = 1f;
    [SerializeField] private string mainMenu;
    [SerializeField] private PlayerHealth playerHealth;

    private void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath += TimerDefeatCinematic;
        }

        containerDefeat.SetActive(false);
    }

    private void TimerDefeatCinematic()
    {
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        for (int second = 0; second < timeBeforeShowDefeatMenu; second++) {
            yield return new WaitForSeconds(1f);
        }
        ShowDefeatMenu();
    }

    private void ShowDefeatMenu()
    {
        Time.timeScale = 0;
        containerDefeat.SetActive(true);
        PauseMenu.isGamePaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        PauseMenu.isGamePaused = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(mainMenu);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath -= TimerDefeatCinematic;
        }
    }
}
