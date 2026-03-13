using System.Collections;
using UnityEngine;

public class DissolverGround : MonoBehaviour
{
    [SerializeField] private float dissolveDuration = 1.2f;
    [SerializeField] private float minDissolveStrength = 0.1f;
    [SerializeField] private float maxDissolveStrength = 0.2f;
    private float dissolveStrength;

    public void Start()
    {
        StartCoroutine(Dissolve());
    }

    public IEnumerator Dissolve() 
    {
        Material material = GetComponentInChildren<Renderer>().material;
        float elapsedTime = 0;
        
        while (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;
            dissolveStrength = Mathf.Lerp(minDissolveStrength, maxDissolveStrength, elapsedTime / dissolveDuration);
            material.SetFloat("_DissolveStrength", dissolveStrength);
            yield return null;
        }
        StartCoroutine(DissolveReverse());
    }

    public IEnumerator DissolveReverse()
    {
        Material material = GetComponentInChildren<Renderer>().material;
        float remainingTime = dissolveDuration;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            dissolveStrength = Mathf.Lerp(minDissolveStrength, maxDissolveStrength, remainingTime / dissolveDuration);
            material.SetFloat("_DissolveStrength", dissolveStrength);
            yield return null;
        }
        StartCoroutine(Dissolve());
    }
}
