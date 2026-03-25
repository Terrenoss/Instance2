using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityBlink : MonoBehaviour
{
    [Header("GameObjects a faire clignoter")]
    public List<GameObject> targetObjects = new List<GameObject>();

    [Header("Materiau d'invincibilite")]
    public Material blinkMaterial;

    [Header("Parametres de l'effet")]
    public float invincibilityDuration = 3f;
    public float blinkFrequency = 10f;
    public bool useVisibilityToggle = false;

    private Dictionary<Renderer, Material[]> _originalMaterials = new Dictionary<Renderer, Material[]>();
    private Coroutine _blinkCoroutine;
    private bool _isBlinking = false;

    public void StartInvincibility()
    {
        if (_isBlinking)
            StopInvincibility();
        _blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    public void StopInvincibility()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
        RestoreAllMaterials();
        _isBlinking = false;
    }
    
    public void RemoveDestroyedObject(GameObject obj)
    {
        targetObjects.Remove(obj);
        
        var keysToRemove = new List<Renderer>();
        foreach (var pair in _originalMaterials)
        {
            if (pair.Key == null || !targetObjects.Contains(pair.Key.gameObject))
                keysToRemove.Add(pair.Key);
        }
        foreach (var key in keysToRemove)
            _originalMaterials.Remove(key);
    }

    private IEnumerator BlinkRoutine()
    {
        _isBlinking = true;
        CacheOriginalMaterials();

        float elapsed = 0f;
        float halfPeriod = 1f / (blinkFrequency * 2f);
        bool showBlink = false;

        while (elapsed < invincibilityDuration)
        {
            showBlink = !showBlink;

            if (showBlink) ApplyBlinkMaterial();
            else RestoreAllMaterials();

            yield return new WaitForSeconds(halfPeriod);
            elapsed += halfPeriod;
        }

        RestoreAllMaterials();
        _isBlinking = false;
    }


    private void CacheOriginalMaterials()
    {
        _originalMaterials.Clear();

        foreach (var go in targetObjects)
        {
            if (go == null) continue;

            foreach (var rend in go.GetComponentsInChildren<Renderer>(true))
            {
                if (_originalMaterials.ContainsKey(rend)) continue;

                Material[] shared = rend.sharedMaterials;
                Material[] saved = new Material[shared.Length];
                System.Array.Copy(shared, saved, shared.Length);
                _originalMaterials[rend] = saved;
            }
        }
    }


    private void ApplyBlinkMaterial()
    {
        if (useVisibilityToggle) { SetRenderersActive(false); return; }

        if (blinkMaterial == null)
        {
            SetRenderersActive(false);
            return;
        }

        foreach (var pair in _originalMaterials)
        {
            Renderer rend = pair.Key;
            if (rend == null) continue;

            Material[] blinkMats = new Material[pair.Value.Length];
            for (int i = 0; i < blinkMats.Length; i++)
                blinkMats[i] = blinkMaterial;

            rend.sharedMaterials = blinkMats;
            rend.enabled = true;
        }
    }

    private void RestoreAllMaterials()
    {
        if (useVisibilityToggle) { SetRenderersActive(true); return; }

        foreach (var pair in _originalMaterials)
        {
            Renderer rend = pair.Key;
            if (rend == null) continue;

            rend.sharedMaterials = pair.Value;
            rend.enabled = true;
        }
    }

    private void SetRenderersActive(bool active)
    {
        foreach (var go in targetObjects)
        {
            if (go == null) continue;
            foreach (var rend in go.GetComponentsInChildren<Renderer>(true))
                rend.enabled = active;
        }
    }

    private void OnDisable()
    {
        StopInvincibility();
    }
}