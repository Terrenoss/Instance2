using System.Collections;
//using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int bestScore = 0;
    [SerializeField] private int passiveScore = 10;
    [SerializeField] private float passiveScoreTimer = 1f;
    [SerializeField] private LevelEnd levelEnd;
    
    [SerializeField] private int parryScore = 200;
    
    //[SerializeField] private int maxMultiplier = 10;
    [SerializeField] private int scoreAdditioner = 1;
    private int actualScoreMultiplier = 1;
    private int baseScoreMultiplier = 1;
    private int bestScoreMultiplier = 1;
    
    //[SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI multiplier2Text;

    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreMultiplierText;
    [SerializeField] private TextMeshProUGUI scoreFinishText;

    //new line
    [SerializeField] private ScoreMultiplierEffect scoreMultiplierEffect;
    [SerializeField] private CasinoScore casinoScore;
    private bool canGainScore = true;
    
    private int actualScore;

    void Start()
    {
        if (levelEnd != null)
        {
            levelEnd.OnMoveFinished += CheckBestScore;
        }
        
        LoadScore();
        StartCoroutine(PassiveScoreRoutine());
        baseScoreMultiplier =  actualScoreMultiplier;
        
        //scoreText.text =  actualScore.ToString();
        multiplierText.text =  actualScoreMultiplier.ToString();
        multiplier2Text.text =  actualScoreMultiplier.ToString();
    }

    IEnumerator PassiveScoreRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(passiveScoreTimer);
            if (canGainScore && levelEnd.canGainScore)
            {
                AddScore(passiveScore);
            }
        }
    }

    private void AddScore(int amount)
    {
        actualScore += amount * actualScoreMultiplier;
        casinoScore.score = actualScore;
        casinoScore.SplitScore();
        //scoreText.text =  actualScore.ToString();
    }

    public void AddParryScore()
    {
        AddScore(parryScore);
    }
    
    public void IncreaseMultiplier()
    {
        actualScoreMultiplier += scoreAdditioner;
        // if (actualScoreMultiplier >= maxMultiplier)
        // {
        //     actualScoreMultiplier = maxMultiplier;
        // }

        if (bestScoreMultiplier < actualScoreMultiplier)
        {
            bestScoreMultiplier = actualScoreMultiplier;
        }
        
        multiplierText.text =  actualScoreMultiplier.ToString();
        //new line
        scoreMultiplierEffect.CheckScoreIncrease(actualScoreMultiplier);
    }

    //call when player take damage
    public void DecreaseMultiplier()
    {
        actualScoreMultiplier = baseScoreMultiplier;
        multiplierText.text =  actualScoreMultiplier.ToString();
        multiplier2Text.text =  actualScoreMultiplier.ToString();
        //new line
        scoreMultiplierEffect.CheckScoreDecrease();
    }

    private void SaveScore()
    {
        SaveableDatas datas = new SaveableDatas("BestScore");

        datas.SaveInt("bestScore", bestScore);

        SaveSystem.SaveData(datas);
    }

    private void LoadScore()
    {
        SaveableDatas datas = SaveSystem.LoadDatas("BestScore");

        if (datas != null)
        {
            bestScore = datas.GetSavedInt("bestScore");
        }
        else
        {
            bestScore = 0;
        }
    }

    public void CheckBestScore()
    {
        if (actualScore > bestScore)
        {
            bestScore = actualScore;
        }
        
        bestScoreText.text = bestScore.ToString();
        bestScoreMultiplierText.text = bestScoreMultiplier.ToString();
        scoreFinishText.text = actualScore.ToString();
        SaveScore();
    }
    
    public void SetCanGainScore(bool canGain)
    {
        canGainScore = canGain;
    }
}
