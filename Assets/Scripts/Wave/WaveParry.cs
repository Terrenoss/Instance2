using System;
using UnityEngine;
public class WaveParry : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private float perfectDistance = 0.4f;
    [SerializeField] private Vector3 boxSize = new(2f, 2f, 2f);
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Renderer feedbackRenderer;
    [SerializeField] private float displayDuration = 0.5f;
    Vector3 center = new();
    private bool parrySuccess = false;

    [SerializeField] private PlayerHealth playerHealth;
    public void Parry(Guid id)
    {
        center = GetParryCenter();
        Collider[] hits = Physics.OverlapBox(transform.position, boxSize / 2, Quaternion.identity);

        if (hits.Length == 0)
        {
            playerHealth.TakeDamage();
            return;
        }
        foreach (Collider hit in hits)
        {
            parrySuccess = false;
            if (hit.TryGetComponent(out WaveType waveType))
            {
                float distance = Vector3.Distance(center, hit.transform.position);

                if (id == waveType.waveParam[waveType.waveTypeEnum].keyId)
                {
                    parrySuccess = true;

                    if (feedbackRenderer != null)
                    {
                        feedbackRenderer.gameObject.SetActive(true);
                        Material feedbackMat = ColorManager.Instance.GetFeedbackMaterial(waveType.waveTypeEnum);
                        if (feedbackMat != null)
                            feedbackRenderer.material = feedbackMat;
                        ParticleSystem ps = feedbackRenderer.GetComponent<ParticleSystem>();
                        if (ps != null)
                            ps.Play();
                    }

                    if (distance <= perfectDistance)
                    {
                        scoreManager.AddParryScore();
                        scoreManager.IncreaseMultiplier();
                    }
                    else
                    {
                        scoreManager.AddParryScore();
                    }
                }
                if (hit.TryGetComponent(out SplineMover waveMover))
                {
                    waveMover.UpdateWave(false);
                }

                break;
            }
        }
        if (!parrySuccess)
        {
            playerHealth.TakeDamage();
        }
    }

    private void HideFeedback()
    {
        if (feedbackRenderer != null)
            feedbackRenderer.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        center = GetParryCenter();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, boxSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, perfectDistance);
    }
    void DrawCube(float size, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(transform.position, new Vector3(size, size, size));
    }

    Vector3 GetParryCenter()
    {
        return transform.position + Vector3.up * yOffset;
    }
}