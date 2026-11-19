
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class JellySound : MonoBehaviour
{
    [Header("Impact Sound")]
    public AudioClip landingSound;

    [Range(0f, 1f)]
    public float volume = 0.5f;

    public float minimumImpact = 1f;

    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasPlayed) return;

        if (landingSound == null) return;

        if (collision.relativeVelocity.magnitude
            < minimumImpact)
            return;

        // Only play when landing from above
        bool landedOnTop = false;

        foreach (ContactPoint2D contact in
                 collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                landedOnTop = true;
                break;
            }
        }

        if (!landedOnTop) return;

        hasPlayed = true;

        audioSource.PlayOneShot(
            landingSound,
            volume
        );
    }
}
