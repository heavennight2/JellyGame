
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class FloorGameOver : MonoBehaviour
{
    [Header("Game Over")]
    public TMP_Text resultText;
    public float gameOverDelay = 0.2f;

    [Header("Music")]
    public AudioSource backgroundMusic;

    private bool gameOver = false;
    private bool isEnding = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isEnding || gameOver) return;

        JellyGrow jelly =
            collision.gameObject.GetComponent<JellyGrow>();

        if (jelly == null) return;

        isEnding = true;
        StartCoroutine(ShowGameOver());
    }

    IEnumerator ShowGameOver()
    {
        yield return new WaitForSeconds(gameOverDelay);

        gameOver = true;

        // Stop background music
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        int finalScore = JellyScore.Instance != null
            ? JellyScore.Instance.score : 0;

        int bestScore = JellyScore.Instance != null
            ? JellyScore.Instance.bestScore : 0;

        // Show Game Over screen
        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);

            resultText.text =
                "GAME OVER!" +
                "\nYOUR SCORE: " + finalScore +
                "\nBEST SCORE: " + bestScore +
                "\nPRESS R TO RESTART";
        }

        Time.timeScale = 0f;
    }

    void Update()
    {
        if (!gameOver || Keyboard.current == null)
            return;

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Time.timeScale = 1f;

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}
