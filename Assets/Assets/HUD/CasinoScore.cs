using UnityEngine;
using TMPro;

public class CasinoScore : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Assign the 9 individual digits here, from Unit (Lowest) to Hundred Millions (Highest)")]
    [SerializeField] private IndividualCasinoScore[] digitScores;
    
    [SerializeField] private RectTransform rectTransformMask;
    [SerializeField] private RectTransform rectTransform;

    public int score = 0;
    private int[] _currentDigits = new int[9];

    [System.Serializable]
    private struct ScoreSizeConfig
    {
        public int threshold;
        public float maskX;
        public float scale;
        public int activeDigits;
    }

    [Header("Layout Configuration")]
    [SerializeField] private ScoreSizeConfig[] sizeConfigs = new ScoreSizeConfig[]
    {
        new ScoreSizeConfig { threshold = 0, maskX = 495, scale = 1.8f, activeDigits = 3 },
        new ScoreSizeConfig { threshold = 1000, maskX = 522, scale = 1.8f, activeDigits = 4 },
        new ScoreSizeConfig { threshold = 10000, maskX = 553, scale = 1.65f, activeDigits = 5 },
        new ScoreSizeConfig { threshold = 100000, maskX = 580, scale = 1.45f, activeDigits = 6 },
        new ScoreSizeConfig { threshold = 1000000, maskX = 599, scale = 1.2f, activeDigits = 7 },
        new ScoreSizeConfig { threshold = 10000000, maskX = 612, scale = 1.1f, activeDigits = 8 },
        new ScoreSizeConfig { threshold = 100000000, maskX = 629, scale = 1f, activeDigits = 9 },
    };

    private void Start()
    {
        SetScore(score);
    }

    public void SetScore(int newScore)
    {
        if (newScore == score) return;
        
        score = newScore;
        
        int tempScore = newScore;
        for (int i = 0; i < digitScores.Length; i++)
        {
            if (digitScores[i] == null) continue;

            int oldDigit = _currentDigits[i];
            int newDigit = tempScore % 10;
            tempScore /= 10;
            
            _currentDigits[i] = newDigit;

            if (oldDigit != newDigit)
            {
                int difference = Mathf.Abs(newDigit - oldDigit);
                // Appel à l'animation originale du chiffre !
                digitScores[i].TriggerAnimatorScroll(difference, oldDigit, newDigit);
            }
            else
            {
                // Pas de changement = pas d'animation
                digitScores[i].Init(newDigit);
            }
        }
        
        UpdateScoreDisplay(score);
    }

    private void UpdateScoreDisplay(int currentScore)
    {
        if (digitScores == null || digitScores.Length == 0) return;

        ScoreSizeConfig bestConfig = sizeConfigs[0];
        for (int i = sizeConfigs.Length - 1; i >= 0; i--)
        {
            if (currentScore >= sizeConfigs[i].threshold)
            {
                bestConfig = sizeConfigs[i];
                break;
            }
        }

        if (rectTransformMask != null)
        {
            Vector2 maskPos = rectTransformMask.anchoredPosition;
            maskPos.x = bestConfig.maskX;
            rectTransformMask.anchoredPosition = maskPos;
        }

        if (rectTransform != null)
        {
            rectTransform.localScale = new Vector3(bestConfig.scale, bestConfig.scale, 1f);
        }

        for (int i = 0; i < digitScores.Length; i++)
        {
            if (digitScores[i] != null)
            {
                bool isActive = i < bestConfig.activeDigits;
                digitScores[i].SetActive(isActive);
            }
        }
    }
}
