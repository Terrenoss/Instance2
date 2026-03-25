using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreMultiplierEffect : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI scoreMultiplier;
    //public int scoreMultiplierInt;

    private void Start()
    {
        animator.SetInteger("scoreMultiplicator", 1);

        StartCoroutine(WaitALitle());
        StartCoroutine(WaitAgain());
    }

    public IEnumerator WaitALitle()
    {
        float elapsedTime = 0;
        while (elapsedTime < 2.5f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        CheckScoreIncrease(55);
    }

    public IEnumerator WaitAgain()
    {
        float elapsedTime = 0;
        while (elapsedTime < 5f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        CheckScoreIncrease(555);
    }

    private void Update()
    {
        //Debug.Log(rectTransform.anchoredPosition);
    }


    public void CheckScoreIncrease(int scoreMultiplierInt)
    {
        if (scoreMultiplierInt > 99) {
            animator.SetInteger("scoreMultiplicator", 100);
            //rectTransform.anchoredPosition = new Vector3(792, 456);
            //rectTransform.localScale = new Vector2(0.75f, 0.75f);
            Debug.Log(rectTransform.anchoredPosition);
        }
        else if (scoreMultiplierInt > 9) {
            animator.SetInteger("scoreMultiplicator", 10);
            //rectTransform.anchoredPosition = new Vector3(810, 460);
            //rectTransform.localScale = new Vector2(0.85f, 0.85f);
            Debug.Log(rectTransform.anchoredPosition);
        }
        else {
            animator.SetInteger("scoreMultiplicator", 1);
            //rectTransform.anchoredPosition = new Vector3(825, 467);
            //rectTransform.localScale = new Vector2(1f, 1f);
            Debug.Log(rectTransform.anchoredPosition);
        }
    }

    public void CheckScoreDecrease(int scoreMultiplierInt)
    {
        if (scoreMultiplierInt > 99) {
            rectTransform.anchoredPosition = new Vector3(792, 456);
            rectTransform.localScale = new Vector2(0.7f, 0.7f);
        }
        else if (scoreMultiplierInt > 9) {
            rectTransform.anchoredPosition = new Vector3(810, 460);
            rectTransform.localScale = new Vector2(0.85f, 0.85f);
        }
        else {
            rectTransform.anchoredPosition = new Vector3(825, 467);
            rectTransform.localScale = new Vector2(1f, 1f);
        }
    }

    public void RestartScoreMultiplicator()
    {
        animator.SetInteger("scoreMultiplicator", 0);
    }
}
