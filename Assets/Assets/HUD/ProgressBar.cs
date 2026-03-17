using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image image;
    private float musicDuration = 5f;

    private void Start()
    {
        StartCoroutine(UpdateProgressBar());
    }

    public IEnumerator UpdateProgressBar()
    {
        float elapsedTime = 0;
        while (elapsedTime < musicDuration)
        {
            elapsedTime += Time.deltaTime;
            image.fillAmount = elapsedTime / musicDuration;
            yield return null;
        }
    }
}
