using System.Collections;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int passiveScore = 10;
    [SerializeField] private float passiveScoreTimer = 1f;
    
    [SerializeField] private int parryScore = 200;
    
    [SerializeField] private int scoreMultiplier = 2;
    [SerializeField] private int actualScoreMultiplier = 1;
    private int baseScoreMultiplier = 1;
    
    private int actualScore;

    void Start()
    {
        StartCoroutine(PassiveScoreRoutine());
        baseScoreMultiplier =  actualScoreMultiplier;
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
    }

    public void AddParryScore()
    {
        AddScore(parryScore);
    }
    
    public void IncreaseMultiplier()
    {
        actualScoreMultiplier *= scoreMultiplier;
    }

    //call when player take damage
    public void DecreaseMultiplier()
    {
        actualScoreMultiplier = baseScoreMultiplier;
    }
}
