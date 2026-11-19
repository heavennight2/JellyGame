
using UnityEngine;
using TMPro;

public class JellyTimer : MonoBehaviour
{
    public static JellyTimer Instance;

    [Header("Timer")]
    public float gameDuration = 60f;
    public TMP_Text timerText;

    [Header("Result")]
    public TMP_Text resultText;

    private float timeLeft;
    private bool gameEnded = false;

    public bool IsFinished => gameEnded;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        timeLeft = gameDuration;

        if (resultText != null)
            resultText.gameObject.SetActive(false);

        UpdateTimerUI();
    }

    void Update()
    {
        if (gameEnded) return;

        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Max(0f, timeLeft);

        UpdateTimerUI();

        if (timeLeft <= 0f)
        {
            EndGame(true);
        }
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int seconds = Mathf.CeilToInt(timeLeft);

        timerText.text = string.Format(
            "{0:00}:{1:00}",
            seconds / 60,
            seconds % 60
        );
    }

    // Player reaches the target score
    public void EndWin()
    {
        FinishGame("YOU WIN!");
    }

    // Called when time runs out or jelly falls
    public void EndGame(bool timeUp)
    {
        if (timeUp)
            FinishGame("TIME'S UP!");
        else
            FinishGame("GAME OVER!");
    }

    void FinishGame(string message)
    {
        if (gameEnded) return;

        gameEnded = true;

        int finalScore = 0;

        if (JellyScore.Instance != null)
            finalScore = JellyScore.Instance.score;

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);

            resultText.text =
                message +
                "\nFINAL SCORE: " +
                finalScore;
        }

        Debug.Log(message);

        Time.timeScale = 0f;
    }
}
