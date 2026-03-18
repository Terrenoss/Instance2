using System.Collections;
using UnityEngine;

public class Dissolver : MonoBehaviour
{
    [SerializeField] private float dissolveDuration = 1.2f;
    private float dissolveStrength;

    public void StartDisolving()
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
            dissolveStrength = Mathf.Lerp(0, 1, elapsedTime / dissolveDuration);
            material.SetFloat("_DissolveStrength", dissolveStrength);

            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + elapsedTime / dissolveDuration / 20,
                gameObject.transform.localScale.y + elapsedTime / dissolveDuration / 20,
                gameObject.transform.localScale.z + elapsedTime / dissolveDuration / 20);

            gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                                                     gameObject.transform.position.y - elapsedTime / dissolveDuration / 40,
                                                     gameObject.transform.position.z);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.GetComponent<DissolverDetec>())
        {
           StartDisolving();
        }
    }
}
