
using UnityEngine;

public class JellyGrow : MonoBehaviour
{
    [Header("Growth Settings")]
    public float growthSpeed = 2.5f;
    public float maxHeight = 4.5f;

    public float GrownHeight { get; private set; }

    private BoxCollider2D box;
    private float startingHeight;

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();

        if (box != null)
            startingHeight =
                box.size.y * Mathf.Abs(transform.localScale.y);
    }

    public float CurrentHeight
    {
        get
        {
            if (box == null) return 0f;

            return box.size.y *
                   Mathf.Abs(transform.localScale.y);
        }
    }

    public void Grow()
    {
        if (box == null) return;

        float remaining = maxHeight - CurrentHeight;

        if (remaining <= 0f) return;

        float amount = Mathf.Min(
            growthSpeed * Time.deltaTime,
            remaining
        );

        Vector3 scale = transform.localScale;
        scale.y += amount / box.size.y;
        transform.localScale = scale;

        GrownHeight = Mathf.Max(
            0f,
            CurrentHeight - startingHeight
        );
    }
}
