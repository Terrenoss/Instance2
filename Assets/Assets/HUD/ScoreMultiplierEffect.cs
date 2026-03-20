using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreMultiplierEffect : MonoBehaviour
{
    [SerializeField] private Animator animator;
    //[SerializeField] private TMP_Text scoreText;

    private void Start()
    {
        animator.SetInteger("scoreMultiplicator", 1);
        animator.SetInteger("scoreMultiplicator", 2);

        StartCoroutine(WaitALitle());
    }

    public IEnumerator WaitALitle()
    {
        float elapsedTime = 0;
        while (elapsedTime < 0.7f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        animator.SetInteger("scoreMultiplicator", 3);
    }

    public void RestartScoreMultiplicator()
    {
        animator.SetInteger("scoreMultiplicator", 1);
    }
}
