
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

    private List<Rigidbody2D> droppedJellies =
        new List<Rigidbody2D>();

    private float timer;
    private float currentSpawnY;
    private float highestTowerTop;
    private float startingCameraY;
    private float targetCameraY;

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
        if (currentJelly == null)
            return;

        timer += Time.deltaTime;

        float x = Mathf.Sin(timer * moveSpeed)
                  * moveRange;

        currentJelly.transform.position =
            new Vector3(x, currentSpawnY, 0f);

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            DropJelly();
        }
    }

    void LateUpdate()
    {
        if (mainCamera == null)
            return;

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
            new Vector3(0, currentSpawnY, 0),
            Quaternion.identity
        );

        currentRB =
            currentJelly.GetComponent<Rigidbody2D>();

        if (currentRB == null)
        {
            Debug.LogError("Jelly needs Rigidbody2D!");
            Destroy(currentJelly);
            currentJelly = null;
            return;
        }

        currentRB.simulated = false;
        currentRB.linearVelocity = Vector2.zero;
        currentRB.angularVelocity = 0f;

        timer = 0f;
        waitingForNext = false;
    }

    void DropJelly()
    {
        if (waitingForNext || currentJelly == null)
            return;

        Rigidbody2D droppedRB = currentRB;

        droppedRB.bodyType = RigidbodyType2D.Dynamic;
        droppedRB.linearVelocity = Vector2.zero;
        droppedRB.angularVelocity = 0f;
        droppedRB.simulated = true;

        droppedJellies.Add(droppedRB);

        currentJelly = null;
        currentRB = null;
        waitingForNext = true;

        StartCoroutine(PrepareNextJelly(droppedRB));
    }

    IEnumerator PrepareNextJelly(Rigidbody2D droppedRB)
    {
        yield return new WaitForSeconds(spawnDelay);

        float elapsed = 0f;
        float stableTime = 0f;

        // Give the dropped jelly time to settle
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
                break;
        }

        UpdateTowerHeight();

        targetCameraY = Mathf.Max(
            startingCameraY,
            highestTowerTop - cameraOffset
        );

        // Wait until the camera reaches the new height
        while (mainCamera != null &&
               Mathf.Abs(
                   mainCamera.transform.position.y -
                   targetCameraY
               ) > 0.05f)
        {
            yield return null;
        }

        SpawnJelly();
    }

    void UpdateTowerHeight()
    {
        foreach (Rigidbody2D rb in droppedJellies)
        {
            if (rb == null)
                continue;

            Collider2D col =
                rb.GetComponent<Collider2D>();

            if (col == null || !col.enabled)
                continue;

            // Ignore pieces that are still flying
            if (rb.linearVelocity.magnitude > 0.5f)
                continue;

            // Ignore pieces far outside the tower
            if (Mathf.Abs(rb.position.x) > 2f)
                continue;

            // Only count pieces touching something
            if (!col.IsTouchingLayers())
                continue;

            highestTowerTop = Mathf.Max(
                highestTowerTop,
                col.bounds.max.y
            );
        }
    }
}
