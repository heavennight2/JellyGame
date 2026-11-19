
using UnityEngine;
using System.Collections;

public class JellyWobble : MonoBehaviour
{
    [Header("Visual")]
    public Transform visual;

    [Header("Jelly Effect")]
    public float squishAmount = 0.25f;
    public float wobbleDuration = 0.65f;
    public float wobbleSpeed = 11f;
    public float minimumImpact = 0.5f;

    private Vector3 originalScale;
    private Coroutine wobbleCoroutine;

    void Awake()
    {
        if (visual == null)
            visual = transform.Find("Visual");

        if (visual == null)
        {
            Debug.LogError("JellyWobble: Visual not found!");
            enabled = false;
            return;
        }

        originalScale = visual.localScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Only wobble the jelly landing from above
        if (collision.transform.position.y >=
            transform.position.y)
            return;

        // Ignore very small impacts
        if (collision.relativeVelocity.magnitude < minimumImpact)
            return;

        if (wobbleCoroutine != null)
            StopCoroutine(wobbleCoroutine);

        wobbleCoroutine = StartCoroutine(Wobble());
    }

    IEnumerator Wobble()
    {
        float elapsed = 0f;

        while (elapsed < wobbleDuration)
        {
            float progress =
                Mathf.Clamp01(elapsed / wobbleDuration);

            // Smoothly fading oscillation
            float damping = Mathf.Pow(1f - progress, 2f);
            float wave = Mathf.Cos(elapsed * wobbleSpeed);

            float amount = squishAmount * wave * damping;

            visual.localScale = new Vector3(
                originalScale.x * (1f + amount),
                originalScale.y * (1f - amount),
                originalScale.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        visual.localScale = originalScale;
        wobbleCoroutine = null;
    }
}
