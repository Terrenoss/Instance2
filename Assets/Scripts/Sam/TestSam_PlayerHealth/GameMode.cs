using UnityEngine;

public class GameMode : MonoBehaviour
{
    private void Start()
    {
        EventManager.Instance.PlayerDeath += GameOver;
    }

    private void GameOver()
    {
        Debug.Log("GameOver");
    }

    private void OnDestroy()
    {
        EventManager.Instance.PlayerDeath -= GameOver;
    }
}
