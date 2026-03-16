using UnityEngine;
using TMPro;

public class PlayerHealthText : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TMP_Text playerHealthText;

    private void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerHit += UpdateHeathText;
        }
        UpdateHeathText();
    }

    public void UpdateHeathText()
    {
        if (playerHealth != null)
        {
            playerHealthText.text = "Health: " + playerHealth.health;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerHit -= UpdateHeathText;
        }
    }
}
