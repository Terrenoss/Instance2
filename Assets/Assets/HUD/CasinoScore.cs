using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CasinoScore : MonoBehaviour
{
    List<TMP_Text> scores = new List<TMP_Text>();
    [SerializeField] private TMP_Text scoreUnit;
    [SerializeField] private TMP_Text scoreTen;
    [SerializeField] private TMP_Text scoreHundred;

    List<int> scoresInt = new List<int>();
    private int unit = 0;
    private int ten = 0;
    private int hundred = 0;

    List<int> scoresTemp = new List<int>();
    private int unitTemp = 0;
    private int tenTemp = 0;
    private int hundredTemp = 0;

    private int score = 725;

    private void Start()
    {
        scores.Add(scoreUnit);
        scores.Add(scoreTen);
        scores.Add(scoreHundred);

        scoresInt.Add(unit);
        scoresInt.Add(ten);
        scoresInt.Add(hundred);

        scoresTemp.Add(unitTemp);
        scoresTemp.Add(tenTemp);
        scoresTemp.Add(hundredTemp);

        int index = 0;
        for (int i = 10; score > i / 10; i *= 10)
        {
            scoresInt[index] = (score % i - score % (i / 10)) / (i / 10);
            scores[index].text = "" + scoresInt[index];
            index++;
        }
        StartCoroutine(WaitALitle());
        //StartCoroutine(WaitAgain());
        //StartCoroutine(WaitAgain2());
        //StartCoroutine(WaitAgain3());
    }

    public IEnumerator WaitALitle()
    {
        float elapsedTime = 0;
        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        score = 929;
        SplitScore();
    }

    public IEnumerator WaitAgain()
    {
        float elapsedTime = 0;
        while (elapsedTime < 1.5)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        score = 539;
        SplitScore();
    }

    public IEnumerator WaitAgain2()
    {
        float elapsedTime = 0;
        while (elapsedTime < 2)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        score = 449;
        SplitScore();
    }

    public IEnumerator WaitAgain3()
    {
        float elapsedTime = 0;
        while (elapsedTime < 2.5)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        score = 359;
        SplitScore();
    }

    private void SplitScore()
    {
        int index = 0;

        for (int i = 0; i < scoresTemp.Count; i++) {
            scoresTemp[i] = scoresInt[i];
        }
        for (int i = 10; score > i / 10; i *= 10) {
            scoresInt[index] = (score % i - score % (i / 10)) / (i / 10);
            index++;
        }
        CheckScoreToAnimated();
    }

    private void CheckScoreToAnimated()
    {
        for (int i = 0; i < scoresInt.Count; i++)
        {
            if (scoresInt[i] != scoresTemp[i])
            {
                int difference = scoresInt[i] - scoresTemp[i];
                if (difference < 0) {
                    difference *= -1;
                }
                int initDifference = difference;
                Debug.Log("scoresInt[i]: " + scoresInt[i]);
                Debug.Log("scoresTemp[i]: " + scoresTemp[i]);
                Debug.Log("difference: " + difference);

                StartCoroutine(ScoreAnimationPart1(i, difference, initDifference));
            }
            else {
                scores[i].text = "" + scoresInt[i];
            }
        }
    }

    public IEnumerator ScoreAnimationPart1(int whichScore, int difference, int initDifference)
    {
        float scoreAnimSpeed = 1f / initDifference;


        // PROBLEME La difference entre les deux valeurs de score est de 2 et de 4, c'est pourquoi l'un
        // descends 2x plus vite que l'autre


        float elapsedTime = 0;
        //while (elapsedTime < 0.5f)
        while (elapsedTime < scoreAnimSpeed)
        {
            scores[whichScore].rectTransform.anchoredPosition = new Vector2(
                scores[whichScore].rectTransform.anchoredPosition.x,
                scores[whichScore].rectTransform.anchoredPosition.y - (800 * scoreAnimSpeed) * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        scores[whichScore].text = "" + (scoresInt[whichScore] - (difference - 1)); // mauvais calcul peut etre

        StartCoroutine(ScoreAnimationPart2(whichScore, difference, scoreAnimSpeed, initDifference));
    }

    public IEnumerator ScoreAnimationPart2(int whichScore, int difference, float scoreAnimSpeed, int initDifference)
    {
        scores[whichScore].rectTransform.anchoredPosition = new Vector2(
            scores[whichScore].rectTransform.anchoredPosition.x,
            scores[whichScore].rectTransform.anchoredPosition.y + 400 * scoreAnimSpeed);

        float elapsedTime = 0;
        //while (elapsedTime < 0.5f)
        while (elapsedTime < scoreAnimSpeed)
            {
            scores[whichScore].rectTransform.anchoredPosition = new Vector2(
                scores[whichScore].rectTransform.anchoredPosition.x,
                scores[whichScore].rectTransform.anchoredPosition.y - (800 * scoreAnimSpeed) * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        difference -= 1;
        if (difference > 0) {
            StartCoroutine(ScoreAnimationPart1(whichScore, difference, initDifference));
        }
    }
}
