using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int bestScore = 0;
    [SerializeField] private int passiveScore = 10;
    [SerializeField] private float passiveScoreTimer = 1f;
    [SerializeField] private Spawner spawner;
    
    [SerializeField] private int parryScore = 200;
    
    [SerializeField] private int maxMultiplier = 10;
    [SerializeField] private int scoreMultiplier = 2;
    [SerializeField] private int actualScoreMultiplier = 1;
    private int baseScoreMultiplier = 1;
    
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    
    private int actualScore;

    void Start()
    {
        if (spawner != null)
        {
            spawner.OnVictory += CheckBestScore;
        }
        
        LoadScore();
        StartCoroutine(PassiveScoreRoutine());
        baseScoreMultiplier =  actualScoreMultiplier;
        Debug.Log("Best Score: " + bestScore);
        
        scoreText.text =  actualScore.ToString();
        multiplierText.text =  actualScoreMultiplier.ToString();
    }

    IEnumerator PassiveScoreRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(passiveScoreTimer);
            AddScore(passiveScore);
        }
    }

    private void AddScore(int amount)
    {
        actualScore += amount * actualScoreMultiplier;
        scoreText.text =  actualScore.ToString();
    }

    public void AddParryScore()
    {
        AddScore(parryScore);
    }
    
    public void IncreaseMultiplier()
    {
        actualScoreMultiplier *= scoreMultiplier;
        if (actualScoreMultiplier >= maxMultiplier)
        {
            actualScoreMultiplier = maxMultiplier;
        }
        multiplierText.text =  actualScoreMultiplier.ToString();
    }

    //call when player take damage
    public void DecreaseMultiplier()
    {
        actualScoreMultiplier = baseScoreMultiplier;
        multiplierText.text =  actualScoreMultiplier.ToString();
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

    private void CheckBestScore()
    {
        if (actualScore > bestScore)
        {
            bestScore = actualScore;
        }
        SaveScore();
        spawner.OnVictory -= CheckBestScore;
        Debug.Log("Final Score: " + bestScore);
        Debug.Log("hi");
    }
}
