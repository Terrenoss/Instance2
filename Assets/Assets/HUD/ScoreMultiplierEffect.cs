using UnityEngine;

public class ScoreMultiplierEffect : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private AudioManager audioManager;

    private void Start()
    {
        if (animator != null)
        {
            animator.SetInteger("scoreMultiplicator", 0);
        }
    }

    public void CheckScoreIncrease(int scoreMultiplierInt)
    {
        if (animator == null) return;

        if (scoreMultiplierInt > 99) 
        {
            animator.SetInteger("scoreMultiplicator", 100);
        }
        else if (scoreMultiplierInt > 9) 
        {
            animator.SetInteger("scoreMultiplicator", 10);
        }
        else 
        {
            animator.SetInteger("scoreMultiplicator", 1);
        }

        if (audioManager != null) audioManager.PlaySound("Multiplicateur");
    }

    public void CheckScoreDecrease()
    {
        if (animator != null)
        {
            animator.SetInteger("scoreMultiplicator", -1);
        }
    }

    public void RestartScoreMultiplicator()
    {
        if (animator != null)
        {
            animator.SetInteger("scoreMultiplicator", 0);
        }
    }
}
