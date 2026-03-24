using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CasinoScore : MonoBehaviour
{
    List<IndividualCasinoScore> individualCasinoScores = new List<IndividualCasinoScore>();
    [SerializeField] private IndividualCasinoScore unitCasinoScore;
    [SerializeField] private IndividualCasinoScore tenCasinoScore;
    [SerializeField] private IndividualCasinoScore hundredCasinoScore;
    [SerializeField] private IndividualCasinoScore thousandCasinoScore;
    [SerializeField] private IndividualCasinoScore tenThousandCasinoScore;
    [SerializeField] private IndividualCasinoScore hundredThousandCasinoScore;
    [SerializeField] private IndividualCasinoScore millionCasinoScore;
    [SerializeField] private IndividualCasinoScore tenMillionCasinoScore;
    [SerializeField] private IndividualCasinoScore hundredMillionCasinoScore;

    List<TMP_Text> scores = new List<TMP_Text>();
    [SerializeField] private TMP_Text scoreUnit;
    [SerializeField] private TMP_Text scoreTen;
    [SerializeField] private TMP_Text scoreHundred;
    [SerializeField] private TMP_Text scoreThousand;
    [SerializeField] private TMP_Text scoreTenThousand;
    [SerializeField] private TMP_Text scoreHundredThousand;
    [SerializeField] private TMP_Text scoremillion;
    [SerializeField] private TMP_Text scoreTenMillion;
    [SerializeField] private TMP_Text scoreHundredMillion;

    List<int> scoresInt = new List<int>();
    private int unit = 0;
    private int ten = 0;
    private int hundred = 0;
    private int thousand = 0;
    private int tenThousand = 0;
    private int hundredThousand = 0;
    private int million = 0;
    private int tenMillion = 0;
    private int hundredMillion = 0;

    List<int> scoresTemp = new List<int>();
    private int unitTemp = 0;
    private int tenTemp = 0;
    private int hundredTemp = 0;
    private int thousandTemp = 0;
    private int tenThousandTemp = 0;
    private int hundredThousandTemp = 0;
    private int millionTemp = 0;
    private int tenMillionTemp = 0;
    private int hunrdedMillionTemp = 0;

    public int score = 725;


    private void Start()
    {
        scores.Add(scoreUnit);
        scores.Add(scoreTen);
        scores.Add(scoreHundred);
        scores.Add(scoreThousand);
        scores.Add(scoreTenThousand);
        scores.Add(scoreHundredThousand);
        scores.Add(scoremillion);
        scores.Add(scoreTenMillion);
        scores.Add(scoreHundredMillion);

        scoresInt.Add(unit);
        scoresInt.Add(ten);
        scoresInt.Add(hundred);
        scoresInt.Add(thousand);
        scoresInt.Add(tenThousand);
        scoresInt.Add(hundredThousand);
        scoresInt.Add(million);
        scoresInt.Add(tenMillion);
        scoresInt.Add(hundredMillion);

        scoresTemp.Add(unitTemp);
        scoresTemp.Add(tenTemp);
        scoresTemp.Add(hundredTemp);
        scoresTemp.Add(thousandTemp);
        scoresTemp.Add(tenThousandTemp);
        scoresTemp.Add(hundredThousandTemp);
        scoresTemp.Add(millionTemp);
        scoresTemp.Add(tenMillionTemp);
        scoresTemp.Add(hunrdedMillionTemp);

        individualCasinoScores.Add(unitCasinoScore);
        individualCasinoScores.Add(tenCasinoScore);
        individualCasinoScores.Add(hundredCasinoScore);
        individualCasinoScores.Add(thousandCasinoScore);
        individualCasinoScores.Add(tenThousandCasinoScore);
        individualCasinoScores.Add(hundredThousandCasinoScore);
        individualCasinoScores.Add(millionCasinoScore);
        individualCasinoScores.Add(tenMillionCasinoScore);
        individualCasinoScores.Add(hundredMillionCasinoScore);

        int index = 0;
        for (int i = 10; score > i / 10; i *= 10)
        {
            scoresInt[index] = (score % i - score % (i / 10)) / (i / 10);
            scores[index].text = "" + scoresInt[index];
            index++;
        }
        StartCoroutine(WaitALitle());
        StartCoroutine(WaitAgain());
        StartCoroutine(WaitAgain2());
        StartCoroutine(WaitAgain3());
    }

    public IEnumerator WaitALitle()
    {
        float elapsedTime = 0;
        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        score = 8726;
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
        score = 895245;
        SplitScore();
    }

    public IEnumerator WaitAgain2()
    {
        float elapsedTime = 0;
        while (elapsedTime < 3.42)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        score = 97248087;
        SplitScore();
    }

    public IEnumerator WaitAgain3()
    {
        float elapsedTime = 0;
        while (elapsedTime < 3.97)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        score = 197248087;
        SplitScore();
    }

    private void SplitScore()
    {
        int index = 0;

        for (int i = 0; i < scoresTemp.Count; i++) {
            scoresTemp[i] = scoresInt[i];
        }
        for (int i = 10; score > i / 10; i *= 10)
        {
            scoresInt[index] = (score % i - score % (i / 10)) / (i / 10);
            Debug.Log("scoresInt[index]: " + scoresInt[index]);
            Debug.Log("index: " + index);
            index++;
        }
        CheckScoreToAnimate();
    }

    private void CheckScoreToAnimate()
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

                ScoreScrolling(i, difference, scoresTemp[i]);
            }
            else {
                scores[i].text = "" + scoresInt[i];
            }
        }
    }

    public void ScoreScrolling(int whichScore, int difference, int scoreTemp)
    {
        individualCasinoScores[whichScore].specialDifference = 10 - scoreTemp;
        individualCasinoScores[whichScore].difference = difference;
        individualCasinoScores[whichScore].scoreTemp = scoreTemp;
        individualCasinoScores[whichScore].scoreValue = (scoresInt[whichScore]);
        individualCasinoScores[whichScore].scorePositiveAgain = false;
        individualCasinoScores[whichScore].isBegining = true;

        individualCasinoScores[whichScore].IndividualScoreScrolling();
    }



    // - - - - - -  - - - - - - - - - - - - - - - - - - - - - - - - 



    //public IEnumerator ScoreAnimationPart1(int whichScore, int difference, int initDifference)
    //{
    //    float scoreAnimSpeed = 1f / initDifference;


    //    // PROBLEME La difference entre les deux valeurs de score est de 2 et de 4, c'est pourquoi l'un
    //    // descends 2x plus vite que l'autre


    //    float elapsedTime = 0;
    //    //while (elapsedTime < 0.5f)
    //    while (elapsedTime < scoreAnimSpeed)
    //    {
    //        scores[whichScore].rectTransform.anchoredPosition = new Vector2(
    //            scores[whichScore].rectTransform.anchoredPosition.x,
    //            scores[whichScore].rectTransform.anchoredPosition.y - (800 * scoreAnimSpeed) * Time.deltaTime);

    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    scores[whichScore].text = "" + (scoresInt[whichScore] - (difference - 1)); // mauvais calcul peut etre

    //    StartCoroutine(ScoreAnimationPart2(whichScore, difference, scoreAnimSpeed, initDifference));
    //}

    //public IEnumerator ScoreAnimationPart2(int whichScore, int difference, float scoreAnimSpeed, int initDifference)
    //{
    //    scores[whichScore].rectTransform.anchoredPosition = new Vector2(
    //        scores[whichScore].rectTransform.anchoredPosition.x,
    //        scores[whichScore].rectTransform.anchoredPosition.y + 400 * scoreAnimSpeed);

    //    float elapsedTime = 0;
    //    //while (elapsedTime < 0.5f)
    //    while (elapsedTime < scoreAnimSpeed)
    //        {
    //        scores[whichScore].rectTransform.anchoredPosition = new Vector2(
    //            scores[whichScore].rectTransform.anchoredPosition.x,
    //            scores[whichScore].rectTransform.anchoredPosition.y - (800 * scoreAnimSpeed) * Time.deltaTime);

    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    difference -= 1;
    //    if (difference > 0) {
    //        StartCoroutine(ScoreAnimationPart1(whichScore, difference, initDifference));
    //    }
    //}
}
