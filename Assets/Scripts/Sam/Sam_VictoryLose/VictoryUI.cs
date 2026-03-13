using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUI : MonoBehaviour
{
    public GameObject containerVictory;
    [SerializeField] private float timeBeforeShowVictoryMenu = 3f;
    [SerializeField] private SceneAsset mainMenu;

    private void Start()
    {
        EventManager.Instance.Victory += TimerVictoryCinematic;

        containerVictory.SetActive(false);
    }

    private void TimerVictoryCinematic()
    {
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        for (int second = 0; second < timeBeforeShowVictoryMenu; second++) {
            yield return new WaitForSeconds(1f);
        }
        ShowVictoryMenu();
    }

    private void ShowVictoryMenu()
    {
        Time.timeScale = 0;
        containerVictory.SetActive(true);
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
        SceneManager.LoadScene(mainMenu.name);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Victory -= TimerVictoryCinematic;
    }
}
