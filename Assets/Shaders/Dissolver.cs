using System.Collections;
using UnityEngine;

public class Dissolver : MonoBehaviour
{
    [SerializeField] private float dissolveDuration = 1.2f;
    private float dissolveStrength;

    public Transform childrenTransform;

    public void StartDisolving()
    {
        StartCoroutine(Dissolve());
    }

    public IEnumerator Dissolve() 
    {
        Material material = GetComponentInChildren<Renderer>().material;
        //Transform childrenTransform = GetComponentInChildren<Transform>();
        float elapsedTime = 0;
        
        while (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;
            dissolveStrength = Mathf.Lerp(0, 1, elapsedTime / dissolveDuration);
            material.SetFloat("_DissolveStrength", dissolveStrength);

            childrenTransform.localScale = new Vector3(childrenTransform.localScale.x + elapsedTime / dissolveDuration / 20,
                                                       childrenTransform.localScale.y + elapsedTime / dissolveDuration / 20,
                                                       childrenTransform.localScale.z + elapsedTime / dissolveDuration / 20);

            childrenTransform.position = new Vector3(childrenTransform.position.x,
                                                     childrenTransform.position.y - elapsedTime / dissolveDuration / 40,
                                                     childrenTransform.position.z);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        // to check if collide with the good trigger, addes Audio Source on it
        if (coll.GetComponent<AudioSource>() != null) {
            StartDisolving();
        }
    }
}
