using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelFinishedUI : MonoBehaviour
{
    [SerializeField] private GameObject containerUI;

    [Header("Timer")]
    [SerializeField] private float timeBeforeShowVictoryMenu = 1f;
    [SerializeField] private float timeBeforeShowDefeatMenu = 1f;

    [SerializeField] private string mainMenu;
    [SerializeField] private LevelEnd levelEnd;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PauseMenu pauseMenu;

    [SerializeField] private GameObject victoryImg;
    [SerializeField] private GameObject defeatImg;

    private void Start()
    {
        if (levelEnd != null)
            levelEnd.OnMoveFinished += HandleVictory;

        if (playerHealth != null)
            playerHealth.OnPlayerDeath += HandleDefeat;

        containerUI.SetActive(false);
        victoryImg.SetActive(false);
        defeatImg.SetActive(false);
    }
    
    private void HandleVictory()
    {
        StartCoroutine(StartTimer(timeBeforeShowVictoryMenu, victoryImg));
    }

    private void HandleDefeat()
    {
        StartCoroutine(StartTimer(timeBeforeShowDefeatMenu, defeatImg));
    }
    
    private IEnumerator StartTimer(float delay, GameObject result)
    {
        yield return new WaitForSeconds(delay);
        ShowEndMenu(result);
    }
    
    private void ShowEndMenu(GameObject result)
    {
        Time.timeScale = 0;

        containerUI.SetActive(true);
        result.SetActive(true);

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
        if (levelEnd != null)
            levelEnd.OnMoveFinished -= HandleVictory;

        if (playerHealth != null)
            playerHealth.OnPlayerDeath -= HandleDefeat;
    }
}