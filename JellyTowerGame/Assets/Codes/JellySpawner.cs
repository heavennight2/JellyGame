
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class JellySpawner : MonoBehaviour
{
    [Header("Jelly")]
    public GameObject jellyPrefab;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float moveRange = 1.5f;

    [Header("Spawning")]
    public float spawnY = 3.5f;
    public float spaceAboveTower = 2f;
    public float spawnDelay = 0.5f;

    [Header("Camera")]
    public float cameraOffset = 1.5f;
    public float cameraSpeed = 4f;

    private Camera mainCamera;
    private GameObject currentJelly;
    private Rigidbody2D currentRB;
    private JellyGrow currentGrow;

    private List<Rigidbody2D> droppedJellies =
        new List<Rigidbody2D>();

    private float timer;
    private float currentSpawnY;
    private float originalHeight;
    private float highestTowerTop;
    private float startingCameraY;
    private float targetCameraY;

    private bool holdingSpace;
    private bool waitingForNext;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            enabled = false;
            return;
        }

        startingCameraY =
            mainCamera.transform.position.y;

        targetCameraY = startingCameraY;
        highestTowerTop = -4.25f;

        SpawnJelly();
    }

    void Update()
    {
        if (currentJelly == null ||
            Keyboard.current == null)
            return;

        var space = Keyboard.current.spaceKey;

        // Move the jelly left and right
        timer += Time.deltaTime;

        float x = Mathf.Sin(
            timer * moveSpeed
        ) * moveRange;

        // Hold SPACE to grow before dropping
        if (space.wasPressedThisFrame)
        {
            holdingSpace = true;
        }

        if (holdingSpace &&
            space.isPressed &&
            currentGrow != null)
        {
            currentGrow.Grow();
        }

        // Keep the bottom at the same spawn position
        float extraHeight = 0f;

        if (currentGrow != null)
        {
            extraHeight =
                currentGrow.CurrentHeight -
                originalHeight;
        }

        currentJelly.transform.position =
            new Vector3(
                x,
                currentSpawnY + extraHeight * 0.5f,
                0f
            );

        UpdateCameraTarget();

        // Release SPACE to drop
        if (holdingSpace &&
            space.wasReleasedThisFrame)
        {
            holdingSpace = false;
            DropJelly();
        }
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        Vector3 position =
            mainCamera.transform.position;

        position.y = Mathf.MoveTowards(
            position.y,
            targetCameraY,
            cameraSpeed * Time.deltaTime
        );

        mainCamera.transform.position = position;
    }

    void SpawnJelly()
    {
        if (jellyPrefab == null)
        {
            Debug.LogError("Assign Jelly Prefab!");
            return;
        }

        currentSpawnY = Mathf.Max(
            spawnY,
            highestTowerTop + spaceAboveTower
        );

        currentJelly = Instantiate(
            jellyPrefab,
            new Vector3(0f, currentSpawnY, 0f),
            Quaternion.identity
        );

        currentRB =
            currentJelly.GetComponent<Rigidbody2D>();

        currentGrow =
            currentJelly.GetComponent<JellyGrow>();

        if (currentRB == null ||
            currentGrow == null)
        {
            Debug.LogError(
                "Jelly needs Rigidbody2D and JellyGrow!"
            );

            Destroy(currentJelly);
            currentJelly = null;
            return;
        }

        currentRB.simulated = false;
        currentRB.linearVelocity = Vector2.zero;
        currentRB.angularVelocity = 0f;

        originalHeight = currentGrow.CurrentHeight;

        timer = 0f;
        holdingSpace = false;
        waitingForNext = false;
    }

    void DropJelly()
    {
        if (waitingForNext ||
            currentJelly == null)
            return;

        Rigidbody2D droppedRB = currentRB;

        float grownHeight =
            currentGrow != null
                ? currentGrow.GrownHeight
                : 0f;

        droppedRB.bodyType =
            RigidbodyType2D.Dynamic;

        droppedRB.simulated = true;
        droppedRB.linearVelocity = Vector2.zero;
        droppedRB.angularVelocity = 0f;

        droppedJellies.Add(droppedRB);

        currentJelly = null;
        currentRB = null;
        currentGrow = null;

        waitingForNext = true;

        StartCoroutine(
            PrepareNextJelly(droppedRB, grownHeight)
        );
    }

    IEnumerator PrepareNextJelly(
        Rigidbody2D droppedRB,
        float grownHeight)
    {
        yield return new WaitForSeconds(spawnDelay);

        float elapsed = 0f;
        float stableTime = 0f;
        bool landed = false;

        while (elapsed < 5f)
        {
            yield return new WaitForFixedUpdate();

            elapsed += Time.fixedDeltaTime;

            if (droppedRB == null)
                break;

            Collider2D col =
                droppedRB.GetComponent<Collider2D>();

            bool stable =
                droppedRB.linearVelocity.magnitude < 0.35f &&
                Mathf.Abs(droppedRB.angularVelocity) < 15f &&
                col != null &&
                col.IsTouchingLayers();

            if (stable)
                stableTime += Time.fixedDeltaTime;
            else
                stableTime = 0f;

            if (stableTime >= 0.3f)
            {
                landed = true;
                break;
            }
        }

        // Award growth points after stable landing
        if (landed &&
            droppedRB != null &&
            Mathf.Abs(droppedRB.position.x) <= 2f &&
            JellyScore.Instance != null)
        {
            JellyScore.Instance.AddGrowthPoints(
                grownHeight
            );
        }

        UpdateTowerHeight();

        targetCameraY = Mathf.Max(
            startingCameraY,
            highestTowerTop - cameraOffset
        );

        while (
            mainCamera != null &&
            Mathf.Abs(
                mainCamera.transform.position.y -
                targetCameraY
            ) > 0.05f
        )
        {
            yield return null;
        }

        SpawnJelly();
    }

    void UpdateCameraTarget()
    {
        if (currentJelly == null ||
            mainCamera == null)
            return;

        Collider2D col =
            currentJelly.GetComponent<Collider2D>();

        if (col == null) return;

        float top = col.bounds.max.y;

        targetCameraY = Mathf.Max(
            startingCameraY,
            top - mainCamera.orthographicSize + 1f
        );
    }

    void UpdateTowerHeight()
    {
        foreach (Rigidbody2D rb in droppedJellies)
        {
            if (rb == null) continue;

            Collider2D col =
                rb.GetComponent<Collider2D>();

            if (col == null || !col.enabled)
                continue;

            if (rb.linearVelocity.magnitude > 0.5f)
                continue;

            if (Mathf.Abs(rb.position.x) > 2f)
                continue;

            if (!col.IsTouchingLayers())
                continue;

            highestTowerTop = Mathf.Max(
                highestTowerTop,
                col.bounds.max.y
            );
        }
    }
}
