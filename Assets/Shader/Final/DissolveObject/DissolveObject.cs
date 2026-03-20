using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DissolveObject : MonoBehaviour
{
    [SerializeField] private float noiseStrength = 0.25f;
    [SerializeField] private float objectHeight = 1.0f;
    [SerializeField] private float dissolveSpeed = 0.5f;

    private Material material;

    private float bottomY;
    private float topY;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;

        var bounds = GetComponent<Renderer>().bounds;
        bottomY = bounds.min.y; 
        topY = bounds.max.y; 
    }

    private void Update()
    {
        float t = (Mathf.Sin(Time.time * dissolveSpeed) + 1f) / 2f; 
        float cutoff = Mathf.Lerp(bottomY - noiseStrength, topY + noiseStrength, t);

        SetHeight(cutoff);
    }

    private void SetHeight(float height)
    {
        material.SetFloat("_CutoffHeight", height);
        material.SetFloat("_NoiseStrength", noiseStrength);
    }
}