using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlameManager : MonoBehaviour
{
    [Header("Flame Settings")]
    [SerializeField] private GameObject flamePrefab;
    [SerializeField] private int startingFlames = 3;
    [SerializeField] private int maxActiveFlamesBeforeFail = 10;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Round Settings")]
    [SerializeField] private float roundDuration = 60f;

    [Header("Spawn Area")]
    [SerializeField] private BoxCollider2D spawnArea;

    [Header("First Flame Spawn")]
    [Tooltip("Optional: Where the FIRST flame of the round should spawn. If null, all flames are random.")]
    [SerializeField] private Transform firstFlameSpawnPoint;

    [Header("End of Round")]
    [SerializeField] private ScreenFader screenFader;
    [SerializeField] private string nextSceneName;

    public Action OnRoundWin;
    public Action OnRoundFail;

    private float timeRemaining;
    private float spawnTimer;
    private int activeFlames;
    private bool roundRunning;

    // Tracks whether we've already used the special first-flame spawn
    private bool firstFlameSpawned;

    private void Start()
    {
        Debug.Log("Starting fireGame roundd!");
        BeginRound();
    }

    public void BeginRound()
    {
        timeRemaining = roundDuration;
        spawnTimer = 0f;
        activeFlames = 0;
        roundRunning = true;
        firstFlameSpawned = false;   // reset at the start of every round

        for (int i = 0; i < startingFlames; i++)
            SpawnFlame();
    }

    private void Update()
    {
        if (!roundRunning)
            return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            HandleRoundWin();
            return;
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            TrySpawnFlame();
            spawnTimer = spawnInterval;
        }
    }

    private void TrySpawnFlame()
    {
        if (activeFlames >= maxActiveFlamesBeforeFail)
        {
            HandleRoundFail();
            return;
        }

        SpawnFlame();
    }

    private void SpawnFlame()
    {
        if (flamePrefab == null)
            return;

        Vector2 spawnPos;

        // Use the special first-flame position ONCE per round, if provided
        if (!firstFlameSpawned && firstFlameSpawnPoint != null)
        {
            spawnPos = firstFlameSpawnPoint.position;
            firstFlameSpawned = true;
        }
        else
        {
            spawnPos = GetRandomPointInArea();
        }

        GameObject flameObj = Instantiate(flamePrefab, spawnPos, Quaternion.identity);

        Flame flame = flameObj.GetComponent<Flame>();

        if (flame != null)
            flame.Init(this);

        activeFlames++;
    }

    private Vector2 GetRandomPointInArea()
    {
        if (!spawnArea)
            return new Vector2(0, 0);

        Bounds b = spawnArea.bounds;
        float x = UnityEngine.Random.Range(b.min.x, b.max.x);
        float y = UnityEngine.Random.Range(b.min.y, b.max.y);
        return new Vector2(x, y);
    }

    public void RegisterFlameExtinguished()
    {
        activeFlames = Mathf.Max(0, activeFlames - 1);
    }

    private void HandleRoundWin()
    {
        if (!roundRunning)
            return;

        roundRunning = false;

        Debug.Log("FlameManager: Round won!");
        OnRoundWin?.Invoke();

        ClearExistingFlames();

        GameManager.instance.isNextRoomDark = false;

        if (screenFader != null)
            screenFader.FadeToBlackAndSwitch(nextSceneName);
    }

    private void HandleRoundFail()
    {
        if (!roundRunning)
            return;

        roundRunning = false;

        Debug.Log("FlameManager: Round failed!");
        OnRoundFail?.Invoke();

        ClearExistingFlames();

        GameManager.instance.isNextRoomDark = true;

        if (screenFader != null)
            screenFader.FadeToBlackAndSwitch(nextSceneName);
    }

    private void ClearExistingFlames()
    {
        Flame[] flames = FindObjectsByType<Flame>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        foreach (var flame in flames)
        {
            Destroy(flame.gameObject);
        }
    }

    public float GetTimeRemaining() => timeRemaining;
    public int GetActiveFlameCount() => activeFlames;
}
