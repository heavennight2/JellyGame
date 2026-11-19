
using UnityEngine;
using TMPro;

public class JellyScore : MonoBehaviour
{
    public static JellyScore Instance;

    public int score = 0;
    public int bestScore = 0;

    [Header("UI")]
    public TMP_Text scoreText;

    void Awake()
    {
        Instance = this;

        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        UpdateScoreUI();
    }

    public void AddGrowthPoints(float grownHeight)
    {
        int basePoints = 10;

        int growthPoints = Mathf.FloorToInt(
            (grownHeight + 0.0001f) / 0.25f
        ) * 10;

        score += basePoints + growthPoints;

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }

        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "SCORE: " + score;
    }
}
