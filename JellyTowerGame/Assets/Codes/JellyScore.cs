
using UnityEngine;
using TMPro;

public class JellyScore : MonoBehaviour
{
    public static JellyScore Instance;

    public int score = 0;
    public TMP_Text scoreText;

    void Awake()
    {
        Instance = this;
        UpdateScoreUI();
    }

    public void AddGrowthPoints(float grownHeight)
    {
        int points = Mathf.FloorToInt(
            (grownHeight + 0.0001f) / 0.25f
        ) * 10;

        score += points;
        UpdateScoreUI();

        Debug.Log("+" + points + " POINTS!");
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "SCORE: " + score;
    }
}
