using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    public float minSpawnRate = 0.5f;  // Minimum spawn rate limit
    public float maxBalloonSpeed = 5f;  // Maximum balloon speed limit
    public float difficultyIncreaseInterval = 30f;  // How often the difficulty increases

    private float difficultyTimer;
    private int difficultyLevel = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        difficultyTimer = 0;
    }

    private void Update()
    {
        difficultyTimer += Time.deltaTime;

        // Increase difficulty after a set interval
        if (difficultyTimer >= difficultyIncreaseInterval)
        {
            IncreaseDifficulty();
            difficultyTimer = 0;  // Reset the timer after difficulty increases
        }
    }

    private void IncreaseDifficulty()
    {
        difficultyLevel++;

        // Decrease balloon spawn rate (increasing difficulty)
        BalloonSpawner.Instance.DecreaseSpawnRate(0.1f);

        // Increase balloon speed
        BalloonSpawner.Instance.IncreaseBalloonSpeed(0.2f);

        Debug.Log($"Difficulty increased to level {difficultyLevel}. Spawn rate and balloon speed adjusted.");
    }
}
