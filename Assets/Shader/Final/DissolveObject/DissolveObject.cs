using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DissolveObject : MonoBehaviour
{
    [SerializeField] private float noiseStrength = 5f;   
    [SerializeField] private float dissolveSpeed = 0.5f;

    private Material material;
    private float bottomY;
    private float topY;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
        Bounds bounds = GetComponent<Renderer>().bounds;
        bottomY = bounds.min.y;
        topY = bounds.max.y;
        SetHeight(topY + noiseStrength);
    }

    public void StartDissolve()
    {
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        float elapsed = 0f;
        float duration = GetDuration();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float cutoff = Mathf.Lerp(topY + noiseStrength, bottomY - noiseStrength, t);
            SetHeight(cutoff);

            yield return null;
        }

        SetHeight(bottomY - noiseStrength);
    }

    public float GetDuration()
    {
        return 1f / dissolveSpeed;
    }

    private void SetHeight(float height)
    {
        material.SetFloat("_CutoffHeight", height);
        material.SetFloat("_NoiseStrength", noiseStrength);
    }
}