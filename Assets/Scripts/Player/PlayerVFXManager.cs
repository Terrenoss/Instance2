using System.Collections;
using UnityEngine;

public class PlayerVFXManager : MonoBehaviour
{

    [Header("Spell Circle VFX")]
    [SerializeField] private GameObject spellCirclePrefab;

    [Header("  Scale")]
    [SerializeField] private float spellStartScale = 0f;
    [SerializeField] private float spellTargetScale = 3f;
    [SerializeField] private float spellGrowDuration = 0.5f;

    [Header("  Fade")]
    [SerializeField] private float spellHoldDuration = 0.3f;
    [SerializeField] private float spellFadeDuration = 0.6f;

    private GameObject spellInstance;
    private ParticleSystem spellCirclePS;
    private ParticleSystemRenderer spellRenderer;
    private MaterialPropertyBlock propBlock;
    private Coroutine spellCoroutine;

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        PlaySpellCircle();
    }

    public void PlaySpellCircle()
    {
        Quaternion rot = Quaternion.Euler(90f, 0f, 0f);
        spellInstance = Instantiate(spellCirclePrefab, transform.position, rot);

        spellCirclePS = spellInstance.GetComponent<ParticleSystem>();
        spellRenderer = spellInstance.GetComponent<ParticleSystemRenderer>();

        if (spellCoroutine != null) StopCoroutine(spellCoroutine);
        spellCoroutine = StartCoroutine(SpellCircleRoutine());
    }

    public void StopSpellCircle()
    {
        if (spellInstance == null) return;
        if (spellCoroutine != null) StopCoroutine(spellCoroutine);
        spellCoroutine = StartCoroutine(FadeOutAndDestroySpell());
    }

    private IEnumerator SpellCircleRoutine()
    {
        spellInstance.transform.localScale = Vector3.one * spellStartScale;
        SetSpellAlpha(1f);
        spellCirclePS.Play();

        yield return ScaleTo(spellInstance.transform, spellStartScale, spellTargetScale, spellGrowDuration);

        yield return new WaitForSeconds(spellHoldDuration);

        yield return FadeOutAndDestroySpell();
    }

    private IEnumerator FadeOutAndDestroySpell()
    {
        if (spellInstance == null) yield break;

        spellCirclePS.Stop(false, ParticleSystemStopBehavior.StopEmitting);

        float elapsed = 0f;
        while (elapsed < spellFadeDuration)
        {
            if (spellInstance == null) yield break;
            elapsed += Time.deltaTime;
            SetSpellAlpha(1f - Mathf.Clamp01(elapsed / spellFadeDuration));
            yield return null;
        }

        if (spellInstance != null) Destroy(spellInstance);

        spellInstance = null;
        spellCirclePS = null;
        spellRenderer = null;
        spellCoroutine = null;
    }

    private IEnumerator ScaleTo(Transform target, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = EaseOutCubic(Mathf.Clamp01(elapsed / duration));
            target.localScale = Vector3.one * Mathf.Lerp(from, to, t);
            yield return null;
        }
        target.localScale = Vector3.one * to;
    }

    private void SetSpellAlpha(float alpha)
    {
        spellRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
        spellRenderer.SetPropertyBlock(propBlock);
    }

    private static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
}