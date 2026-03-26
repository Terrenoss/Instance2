using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private const string BestScoreKey = "BestScore";

    [Header("Score Settings")]
    [SerializeField] private int passiveScore = 10;
    [SerializeField] private float passiveScoreTimer = 1f;
    [SerializeField] private int parryScore = 200;
    [SerializeField] private int scoreAdditioner = 1;

    [Header("References")]
    [SerializeField] private ScoreMultiplierEffect scoreMultiplierEffect;
    [SerializeField] private CasinoScore casinoScore;
    
    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI multiplier2Text;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreMultiplierText;
    [SerializeField] private TextMeshProUGUI scoreFinishText;

    private int bestScore = 0;
    private int actualScoreMultiplier = 1;
    private int baseScoreMultiplier = 1;
    private int bestScoreMultiplier = 1;
    private int actualScore = 0;
    private bool canGainScore = true;

    private void Start()
    {
        LevelEnd.GlobalOnMoveFinished += CheckBestScore;
        
        LoadScore();
        baseScoreMultiplier = actualScoreMultiplier;
        
        UpdateMultiplierUI();
        StartCoroutine(PassiveScoreRoutine());
    }

    private void OnDestroy()
    {
        LevelEnd.GlobalOnMoveFinished -= CheckBestScore;
    }

    private IEnumerator PassiveScoreRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(passiveScoreTimer);
            
            if (canGainScore && LevelEnd.GlobalCanGainScore)
            {
                AddScore(passiveScore);
            }
        }
    }

    private void AddScore(int amount)
    {
        actualScore += amount * actualScoreMultiplier;
        if (casinoScore != null)
        {
            casinoScore.SetScore(actualScore);
        }
    }

    public void AddParryScore()
    {
        AddScore(parryScore);
    }
    
    public void IncreaseMultiplier()
    {
        actualScoreMultiplier += scoreAdditioner;
        bestScoreMultiplier = Mathf.Max(bestScoreMultiplier, actualScoreMultiplier);
        
        UpdateMultiplierUI();
        
        if (scoreMultiplierEffect != null)
        {
            scoreMultiplierEffect.CheckScoreIncrease(actualScoreMultiplier);
        }
    }

    public void DecreaseMultiplier()
    {
        actualScoreMultiplier = baseScoreMultiplier;
        UpdateMultiplierUI();
        
        if (scoreMultiplierEffect != null)
        {
            scoreMultiplierEffect.CheckScoreDecrease();
        }
    }

    private void UpdateMultiplierUI()
    {
        string formattedMultiplier = "x" + actualScoreMultiplier;
        
        if (multiplierText != null) multiplierText.text = formattedMultiplier;
        if (multiplier2Text != null) multiplier2Text.text = formattedMultiplier;
    }

    private void SaveScore()
    {
        SaveableDatas datas = new SaveableDatas(BestScoreKey);
        datas.SaveInt("bestScore", bestScore);
        SaveSystem.SaveData(datas);
    }

    private void LoadScore()
    {
        SaveableDatas datas = SaveSystem.LoadDatas(BestScoreKey);
        bestScore = (datas != null) ? datas.GetSavedInt("bestScore") : 0;
    }

    public void CheckBestScore()
    {
        bestScore = Mathf.Max(bestScore, actualScore);
        
        if (bestScoreText != null) bestScoreText.text = bestScore.ToString();
        if (bestScoreMultiplierText != null) bestScoreMultiplierText.text = "x" + bestScoreMultiplier;
        if (scoreFinishText != null) scoreFinishText.text = actualScore.ToString();
        
        SaveScore();
    }
    
    public void SetCanGainScore(bool canGain)
    {
        canGainScore = canGain;
    }
}
