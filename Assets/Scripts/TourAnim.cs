using UnityEngine;

public class TourAnim : MonoBehaviour
{
    Animator animator;

    public void PlayAnim()
    {
        animator.SetBool("Spawn", true);
    }
    
    public void StopAnim()
    {
        animator.SetBool("Spawn", false);
    }
}
