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
    private int hundredMillionTemp = 0;

    [SerializeField] private RectTransform rectTransformMask;
    [SerializeField] private RectTransform rectTransform;
    public int score = 000;


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
        scoresTemp.Add(hundredMillionTemp);

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

        for (int i = 3; i < scores.Count; i++) {
            scores[i].GetComponent<TextMeshProUGUI>().enabled = false;
        }
        rectTransformMask.anchoredPosition = new Vector3(495, 437);
        rectTransform.localScale = new Vector3(1.8f, 1.8f);

        // to test 
        //StartCoroutine(WaitALitle());
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
        score = 100;
        SplitScore();
    }

    public IEnumerator WaitAgain()
    {
        float elapsedTime = 0;
        while (elapsedTime < 2.14)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        score = 1000;
        SplitScore();
    }

    public IEnumerator WaitAgain2()
    {
        float elapsedTime = 0;
        while (elapsedTime < 3.97)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        score = 10100;
        SplitScore();
    }

    public IEnumerator WaitAgain3()
    {
        float elapsedTime = 0;
        while (elapsedTime < 5.75)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        score = 548127576;
        SplitScore();
    }

    public void SplitScore()
    {
        int index = 0;

        for (int i = 0; i < scoresTemp.Count; i++) {
            scoresTemp[i] = scoresInt[i];
        }
        //for (long i = 10; score > i / 10; i *= 10)
        for (long i = 10; score >= i / 10; i *= 10) {
            scoresInt[index] = (int)((score % i - score % (i / 10)) / (i / 10));
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
        individualCasinoScores[whichScore].scoreValue = ((int)scoresInt[whichScore]);
        individualCasinoScores[whichScore].scorePositiveAgain = false;
        individualCasinoScores[whichScore].isBegining = true;

        individualCasinoScores[whichScore].IndividualScoreScrolling();

        CheckScoreSize();
    }

    public void CheckScoreSize()
    {
        if (score > 999 && score < 10000) {
            rectTransformMask.anchoredPosition = new Vector3(522, rectTransformMask.anchoredPosition.y);
            for (int i = 0; i < 4; i++) {
                scores[i].enabled = true;
            }
        }
        else if (score > 9999 && score < 100000) {
            rectTransformMask.anchoredPosition = new Vector3(553, rectTransformMask.anchoredPosition.y);
            rectTransform.localScale = new Vector3(1.65f, 1.65f);
            for (int i = 0; i < 5; i++) {
                scores[i].enabled = true;
            }
        }
        else if (score > 99999 && score < 1000000) {
            rectTransformMask.anchoredPosition = new Vector3(580, rectTransformMask.anchoredPosition.y);
            rectTransform.localScale = new Vector3(1.45f, 1.45f);
            for (int i = 0; i < 6; i++) {
                scores[i].enabled = true;
            }
        }
        else if (score > 999999 && score < 10000000) { 
            rectTransformMask.anchoredPosition = new Vector3(599, rectTransformMask.anchoredPosition.y);
            rectTransform.localScale = new Vector3(1.2f, 1.2f);
            for (int i = 0; i < 7; i++) {
                scores[i].enabled = true;
            }
        }
        else if (score > 9999999 && score < 100000000) {
            rectTransformMask.anchoredPosition = new Vector3(612, rectTransformMask.anchoredPosition.y);
            rectTransform.localScale = new Vector3(1.1f, 1.1f);
            for (int i = 0; i < 8; i++) {
                scores[i].enabled = true;
            }
        }
        else if (score > 99999999 && score < 1000000000) {
            rectTransformMask.anchoredPosition = new Vector3(629, rectTransformMask.anchoredPosition.y);
            rectTransform.localScale = new Vector3(1f, 1f);
            for (int i = 0; i < 9; i++) {
                scores[i].enabled = true;
            }
        }
    }
}
