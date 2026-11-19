
using UnityEngine;

public class JellyScore : MonoBehaviour
{
    public static JellyScore Instance;

    public int score = 0;

    void Awake()
    {
        Instance = this;
    }

    public void AddPoints(int points)
    {
        score += points;
        Debug.Log("SCORE: " + score);
    }
}
