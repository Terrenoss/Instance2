using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelFinishedUI : MonoBehaviour
{
    [SerializeField] private GameObject containerUI;
    [SerializeField] private TMP_Text resultText;

    [Header("Timer")]
    [SerializeField] private float timeBeforeShowVictoryMenu = 1f;
    [SerializeField] private float timeBeforeShowDefeatMenu = 1f;

    [SerializeField] private string mainMenu;
    [SerializeField] private Spawner spawner;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PauseMenu pauseMenu;

    [SerializeField] private string victoryText;
    [SerializeField] private string defeatText;

    private void Start()
    {
        if (spawner != null)
            spawner.OnVictory += HandleVictory;

        if (playerHealth != null)
            playerHealth.OnPlayerDeath += HandleDefeat;

        containerUI.SetActive(false);
    }
    
    private void HandleVictory()
    {
        StartCoroutine(StartTimer(timeBeforeShowVictoryMenu, "YOU WIN !"));
    }

    private void HandleDefeat()
    {
        StartCoroutine(StartTimer(timeBeforeShowDefeatMenu, "YOU LOSE !"));
    }
    
    private IEnumerator StartTimer(float delay, string result)
    {
        yield return new WaitForSeconds(delay);
        ShowEndMenu(result);
    }
    
    private void ShowEndMenu(string result)
    {
        Time.timeScale = 0;

        containerUI.SetActive(true);
        resultText.text = result;

        if (pauseMenu != null)
            pauseMenu.isGamePaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;

        if (pauseMenu != null)
            pauseMenu.isGamePaused = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene(mainMenu);
    }
    
    private void OnDestroy()
    {
        if (spawner != null)
            spawner.OnVictory -= HandleVictory;

        if (playerHealth != null)
            playerHealth.OnPlayerDeath -= HandleDefeat;
    }
}