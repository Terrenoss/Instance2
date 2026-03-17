using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private Image fillImage;

    void Update()
    {
        if (spawner == null) return;

        float levelDuration = spawner.LevelDuration;
        if (levelDuration <= 0f) return;

        float progress = spawner.CurrentTime / levelDuration;
        progress = Mathf.Clamp01(progress);

        fillImage.fillAmount = progress;
    }
}