using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RemoveHealthPoint : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    private int hitTaken = 0;

    private List<Image> healthPoints = new List<Image>();
    [SerializeField] private Image healthPoint1;
    [SerializeField] private Image healthPoint2;
    [SerializeField] private Image healthPoint3;

    [SerializeField] private float destroyDuration = 0.8f;
    private float destroyStrength = -0.5f;


    private void Start()
    {
        if (playerHealth != null) {
            playerHealth.OnPlayerHit += StartDestroying;
        }
        healthPoints.Add(healthPoint1);
        healthPoints.Add(healthPoint2);
        healthPoints.Add(healthPoint3);

        destroyStrength = -0.5f;
        foreach (Image healthpoint in healthPoints)
        {
            healthpoint.enabled = true;
            healthpoint.material.SetFloat("_DestroyStrength", destroyStrength);
        }
    }

    public void StartDestroying()
    {
        StartCoroutine(Destroying());
    }

    public IEnumerator Destroying()
    {
        hitTaken++;
        hitTaken = Mathf.Clamp(hitTaken, 1, 3);

        float elapsedTime = 0;
        while (elapsedTime < destroyDuration)
        {
            elapsedTime += Time.deltaTime;
            destroyStrength = Mathf.Lerp(0.1f, 1, elapsedTime / destroyDuration);
            //Debug.Log(": "+ hitTaken);
            healthPoints[healthPoints.Count - hitTaken].material.SetFloat("_DestroyStrength", destroyStrength);
            //Debug.Log("[healthPoints.Count - hitTaken]: " + (healthPoints.Count - hitTaken));
            yield return null;
        }
        healthPoints[healthPoints.Count - hitTaken].enabled = false;
    }

    private void OnDestroy()
    {
        if (playerHealth != null) {
            playerHealth.OnPlayerHit -= StartDestroying;
        }
    }
}
