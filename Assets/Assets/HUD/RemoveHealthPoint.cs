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

    [SerializeField] private float destroyDuration = 1.2f;
    private float destroyStrength = 0f;

    private void Start()
    {
        if (playerHealth != null) {
            playerHealth.OnPlayerHit += StartDestroying;
        }
        healthPoints.Add(healthPoint1);
        healthPoints.Add(healthPoint2);
        healthPoints.Add(healthPoint3);

        foreach (Image healthpoint in healthPoints) {
            healthpoint.enabled = true;
        }
    }

    public void StartDestroying()
    {
        StartCoroutine(Destroying());
    }

    public IEnumerator Destroying()
    {
        hitTaken++;
        if (hitTaken <= healthPoints.Count)
        {
            Material material = healthPoints[healthPoints.Count - hitTaken].GetComponent<Image>().material;

            float elapsedTime = 0;

            while (elapsedTime < destroyDuration)
            {
                elapsedTime += Time.deltaTime;
                destroyStrength = Mathf.Lerp(0, 1, elapsedTime / destroyDuration);
                material.SetFloat("_DissolveStrength", destroyStrength);
                yield return null;
            }
            healthPoints[healthPoints.Count - hitTaken].enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null) {
            playerHealth.OnPlayerHit -= StartDestroying;
        }
    }
}
