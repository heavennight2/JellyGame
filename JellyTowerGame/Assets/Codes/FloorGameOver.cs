
using UnityEngine;
using System.Collections;

public class FloorGameOver : MonoBehaviour
{
    public float gameOverDelay = 0.8f;

    private bool gameOver = false;

    void Start()
    {
        Time.timeScale = 1f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameOver) return;

        JellyGrow jelly =
            collision.gameObject.GetComponent<JellyGrow>();

        if (jelly == null) return;

        gameOver = true;

        Debug.Log("Jelly touched the floor!");

        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(gameOverDelay);

        Debug.Log("GAME OVER!");

        Time.timeScale = 0f;
    }
}
