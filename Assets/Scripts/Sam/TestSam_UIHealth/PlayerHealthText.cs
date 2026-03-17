using UnityEngine;
using TMPro;

public class PlayerHealthText : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TMP_Text playerHealthText;

    private void Start()
    {
        EventManager.Instance.PlayerHit += UpdateHeathText;
        UpdateHeathText();
    }

    public void UpdateHeathText()
    {
        playerHealthText.text = "Health: " + playerHealth.health;
    }

    private void OnDestroy()
    {
        EventManager.Instance.PlayerHit -= UpdateHeathText;
    }
}
