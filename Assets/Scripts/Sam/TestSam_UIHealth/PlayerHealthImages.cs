using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealthImages : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    
    private List<Image> healthPoints = new List<Image>();
    
    [SerializeField] private Image healthPoint1;
    [SerializeField] private Image healthPoint2;
    [SerializeField] private Image healthPoint3;

    private int hitTaken = 0;

    private void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerHit += UpdateHealthPoint;
        }

        healthPoints.Add(healthPoint1);
        healthPoints.Add(healthPoint2);
        healthPoints.Add(healthPoint3);

        foreach (Image healthpoint in healthPoints) {
            healthpoint.enabled = true;
        }
    }

    private void UpdateHealthPoint()
    {
        hitTaken++;
        if (hitTaken <= healthPoints.Count)
        {
            //healthPoints[healthPoints.Count - hitTaken].enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerHit -= UpdateHealthPoint;
        }
    }
}
