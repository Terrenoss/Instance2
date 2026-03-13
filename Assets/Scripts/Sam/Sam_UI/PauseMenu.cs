using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject containerPause;
    public static bool isGamePaused = false;
    [SerializeField] private SceneAsset mainMenu;

    private void Start()
    {
        containerPause.SetActive(false);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape) && isGamePaused == false) {
        //    PausedGame();
        //}
        //else if (Input.GetKeyDown(KeyCode.Escape) && isGamePaused == true) {
        //    ResumeGame();
        //}
    }

    public void PausedGame()
    {
        Time.timeScale = 0;
        containerPause.SetActive(true);
        isGamePaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        containerPause.SetActive(false);
        isGamePaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ReturnToMainMenu() // répétition avec le start du mainMenu (tout dupliquer par sécurité ?)
    {
        Time.timeScale = 1;
        isGamePaused = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(mainMenu.name);
    }
}
