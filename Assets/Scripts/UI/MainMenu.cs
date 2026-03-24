using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string scene;
    [SerializeField] private GameObject credits;

    private void Start()
    {
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(scene);
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleCredit()
    {
        credits.gameObject.SetActive(!credits.activeSelf);
    }
}
