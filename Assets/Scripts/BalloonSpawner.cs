using UnityEngine;
using UnityEngine.Pool; // For object pooling
using UnityEngine.UI;   // For UI Slider
using System.Collections.Generic; // For List

public class BalloonSpawner : MonoBehaviour
{
    public GameObject[] balloonPrefabs;  // Array of balloon prefabs
    public float spawnRate;         // Time interval for spawning balloons
    public Transform[] spawnPoints;      // Spawn points for the balloons

    private float timer = 0;
    private ObjectPool<GameObject> balloonPool;  // Object pool for balloons
    public Slider balloonCountSlider; // Reference to slider in UI
    private List<GameObject> activeBalloons = new List<GameObject>(); // Active balloon tracker

    public static BalloonSpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the spawner alive across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure no duplicates
        }
    }

    void Start()
    {
        Debug.Log("Balloon Spawner Initialized"); // Debug to check initialization
        // Initialize the object pool
        balloonPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(balloonPrefabs[0]), // Default prefab
            actionOnGet: (balloon) =>
            {
                balloon.SetActive(true);
                activeBalloons.Add(balloon); // Track active balloons
            },
            actionOnRelease: (balloon) =>
            {
                balloon.SetActive(false);
                activeBalloons.Remove(balloon); // Remove from active balloons
            },
            actionOnDestroy: (balloon) => Destroy(balloon),
            defaultCapacity: 10,
            maxSize: 20
        );

        balloonCountSlider.value = Mathf.InverseLerp(0.1f, 3f, 1f);  // Middle point for 1 second

        // Set the listener to adjust spawn rate based on slider value
        balloonCountSlider.onValueChanged.AddListener(OnBalloonCountChanged);

        // Adjust the spawn rate initially based on the slider's starting value
        OnBalloonCountChanged(balloonCountSlider.value);
    }

    void Update()
    {
        timer += Time.deltaTime;  // Increment timer
        if (timer >= spawnRate)
        {
            Debug.Log("Timer triggered spawn"); // Debug timer
            SpawnBalloon();
            timer = 0;  // Reset timer
        }
    }

    void SpawnBalloon()
    {
        Debug.Log("Attempting to spawn a balloon");
        int randomIndex = Random.Range(0, spawnPoints.Length);
        GameObject balloon = balloonPool.Get();  // Get a balloon from the pool
        balloon.transform.position = spawnPoints[randomIndex].position;

        // Set random balloon type
        int balloonType = Random.Range(0, balloonPrefabs.Length);
        balloon.GetComponent<SpriteRenderer>().sprite = balloonPrefabs[balloonType].GetComponent<SpriteRenderer>().sprite;
        balloon.transform.localScale = balloonPrefabs[balloonType].transform.localScale;

        // Set random color
        SpriteRenderer sr = balloon.GetComponent<SpriteRenderer>();
        sr.color = new Color(Random.value, Random.value, Random.value);

        // Set speed for the balloon
        BalloonMovement balloonMovement = balloon.GetComponent<BalloonMovement>();
        balloonMovement.speed = Random.Range(1f, 3f);
    }

    public void ReleaseBalloon(GameObject balloon)
    {
        balloonPool.Release(balloon); // Release balloon back to pool
    }

    public void DecreaseSpawnRate(float amount)
    {
        spawnRate = Mathf.Max(spawnRate - amount, DifficultyManager.Instance.minSpawnRate);
    }

    public void IncreaseBalloonSpeed(float amount)
    {
        foreach (GameObject balloon in activeBalloons)
        {
            if (balloon.activeSelf)
            {
                BalloonMovement balloonMovement = balloon.GetComponent<BalloonMovement>();
                balloonMovement.speed = Mathf.Min(balloonMovement.speed + amount, DifficultyManager.Instance.maxBalloonSpeed);
            }
        }
    }

    void OnBalloonCountChanged(float value)
    {
        spawnRate = Mathf.Lerp(0.2f, 3f, 1f - value); // Change minimum to 0.5f instead

        // Debugging to check the value
        Debug.Log($"Adjusted spawn rate to: {spawnRate} seconds per balloon");
    }

    public void ResetSpawner()
    {
        foreach (GameObject balloon in activeBalloons)
        {
            if (balloon.activeSelf)
            {
                ReleaseBalloon(balloon);  // Release active balloons
            }
        }
        activeBalloons.Clear();  // Clear active balloons list
        timer = 0;
        spawnRate = 1f;  // Reset spawn rate
    }
}
