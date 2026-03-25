using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreMultiplierEffect : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI scoreMultiplier;

    private void Start()
    {
        animator.SetInteger("scoreMultiplicator", 0);

        //StartCoroutine(WaitALitle());
    }

    //public IEnumerator WaitALitle()
    //{
    //    float elapsedTime = 0;
    //    while (elapsedTime < 3f)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    CheckScoreIncrease(55);
    //}


    public void CheckScoreIncrease(int scoreMultiplierInt)
    {
        if (scoreMultiplierInt > 99) {
            animator.SetInteger("scoreMultiplicator", 100);
        }
        else if (scoreMultiplierInt > 9) {
            animator.SetInteger("scoreMultiplicator", 10);
        }
        else {
            animator.SetInteger("scoreMultiplicator", 1);
        }
    }

    public void CheckScoreDecrease()
    {
        animator.SetInteger("scoreMultiplicator", -1);
    }

    public void RestartScoreMultiplicator()
    {
        animator.SetInteger("scoreMultiplicator", 0);
    }
}
