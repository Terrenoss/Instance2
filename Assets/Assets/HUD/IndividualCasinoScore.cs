using TMPro;
using UnityEngine;

public class IndividualCasinoScore : MonoBehaviour
{
    //[SerializeField] private CasinoScore casinoScore;
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text score;
    public int specialDifference = 0;
    public int difference = 0;
    public int scoreTemp = 0;
    public int scoreValue = 0;
    public bool scorePositiveAgain = false;
    public bool isBegining = false;


    public void IndividualScoreScrolling()
    {
        if (isBegining)
        {
            animator.Play("Empty");
            animator.SetInteger("remainingScrolls", 0);
            isBegining = false;
        }
        if (scoreValue <= scoreTemp)
        {
            animator.SetInteger("remainingScrolls", specialDifference);
        }
        else { 
            animator.SetInteger("remainingScrolls", difference);
        }

        difference -= 1;
        specialDifference -= 1;
    }

    public void IndividualScoreUpdate()
    {
        if (scoreValue <= scoreTemp && !scorePositiveAgain)
        {
            if (specialDifference <= 0)
            {
                specialDifference = scoreValue;
                score.text = "" + (scoreValue - specialDifference);
                scorePositiveAgain = true;
            }
            else {
                score.text = "" + (10 - specialDifference);
            }
        }
        else if (scoreValue <= scoreTemp && scorePositiveAgain)
        {
            score.text = "" + (scoreValue - specialDifference);
        }
        else {
            score.text = "" + (scoreValue - difference);
        }
    }
}
