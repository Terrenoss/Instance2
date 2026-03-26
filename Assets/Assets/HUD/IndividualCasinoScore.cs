using UnityEngine;
using TMPro;

public class IndividualCasinoScore : MonoBehaviour
{
    private TMP_Text scoreText;
    private Animator _animator;

    [HideInInspector] public int specialDifference = 0;
    [HideInInspector] public int difference = 0;
    [HideInInspector] public int scoreTemp = 0;
    [HideInInspector] public int scoreValue = 0;
    [HideInInspector] public bool scorePositiveAgain = false;
    [HideInInspector] public bool isBegining = false;

    private void Awake()
    {
        scoreText = GetComponent<TMP_Text>();
        _animator = GetComponent<Animator>();
    }

    public void Init(int startingDigit)
    {
        scoreValue = startingDigit;
        scoreTemp = startingDigit;
        if (scoreText != null) 
        {
            scoreText.text = startingDigit.ToString();
        }
    }

    public void SetActive(bool isActive)
    {
        if (scoreText != null)
        {
            scoreText.enabled = isActive;
        }
    }

    // Nouvelle fonction pour faire le pont exact avec ton ancienne logique animée :
    public void TriggerAnimatorScroll(int diff, int oldDigit, int newDigit)
    {
        difference = diff;
        scoreTemp = oldDigit;
        scoreValue = newDigit;
        specialDifference = 10 - oldDigit;
        scorePositiveAgain = false;
        isBegining = true;
        
        IndividualScoreScrolling();
    }

    // --- CES DEUX MÉTHODES SONT CELLES ORIGINALES APPELÉES PAR TON ANIMATOR ---

    public void IndividualScoreScrolling()
    {
        if (_animator != null)
        {
            if (isBegining)
            {
                _animator.Play("Empty");
                _animator.SetInteger("remainingScrolls", 0);
                isBegining = false;
            }

            if (scoreValue <= scoreTemp)
            {
                _animator.SetInteger("remainingScrolls", specialDifference);
            }
            else { 
                _animator.SetInteger("remainingScrolls", difference);
            }
        }

        difference -= 1;
        specialDifference -= 1;
    }

    public void IndividualScoreUpdate()
    {
        if (scoreText == null) return;

        if (scoreValue <= scoreTemp && !scorePositiveAgain)
        {
            if (specialDifference <= 0)
            {
                specialDifference = scoreValue;
                scoreText.text = "" + (scoreValue - specialDifference);
                scorePositiveAgain = true;
            }
            else {
                scoreText.text = "" + (10 - specialDifference);
            }
        }
        else if (scoreValue <= scoreTemp && scorePositiveAgain)
        {
            scoreText.text = "" + (scoreValue - specialDifference);
        }
        else {
            scoreText.text = "" + (scoreValue - difference);
        }
    }
}
