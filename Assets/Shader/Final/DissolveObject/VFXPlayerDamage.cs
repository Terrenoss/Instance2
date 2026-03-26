using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXPlayerDamage : MonoBehaviour
{
    [System.Serializable]
    public struct BodyPartEntry
    {
        public int lifeThreshold;
        public List<GameObject> bodyParts;
    }

    [SerializeField] private List<BodyPartEntry> _bodyPartsSetup;
    [SerializeField] private Material _dissolveMaterial;  
    [SerializeField] private float _destroyDelay = 2f;
    [SerializeField] private InvincibilityBlink _invincibilityBlink;

    public Dictionary<int, List<GameObject>> dissolveMaterials = new Dictionary<int, List<GameObject>>();

    private void Awake()
    {
        foreach (var entry in _bodyPartsSetup)
            dissolveMaterials[entry.lifeThreshold] = entry.bodyParts;
    }

    public void PlayVfx(int life)
    {
        if (dissolveMaterials.TryGetValue(life, out List<GameObject> parts))
        {
            foreach (GameObject part in parts)
            {
                if (part != null)
                    StartCoroutine(DissolveAndDestroy(part));
            }
        }
    }
    
    private IEnumerator DissolveAndDestroy(GameObject part)
    {
        MeshRenderer renderer = part.GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.material = new Material(_dissolveMaterial);

        DissolveObject dissolve = part.GetComponent<DissolveObject>();
        if (dissolve == null)
            dissolve = part.AddComponent<DissolveObject>();

        yield return null;

        dissolve.StartDissolve();

        yield return new WaitForSeconds(dissolve.GetDuration());
        
        if (_invincibilityBlink != null)
            _invincibilityBlink.RemoveDestroyedObject(part);

        Destroy(renderer.material);
        Destroy(part);
    }
}
