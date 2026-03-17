using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealthImages : MonoBehaviour
{
    private List<Image> healthPoints = new List<Image>();
    [SerializeField] private Image healthPoint1;
    [SerializeField] private Image healthPoint2;
    [SerializeField] private Image healthPoint3;

    private int hitTaken = 0;

    private void Start()
    {
        EventManager.Instance.PlayerHit += UpdateHealthPoint;

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
        healthPoints[healthPoints.Count - hitTaken].enabled = false;
    }

    private void OnDestroy()
    {
        EventManager.Instance.PlayerHit -= UpdateHealthPoint;
    }
}
