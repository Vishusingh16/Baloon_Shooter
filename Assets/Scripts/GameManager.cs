using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes
        }
        else
        {
            Destroy(gameObject);  // Ensure no duplicates
        }
    }

    // Reset score when the Reset button is pressed
    public void ResetScore()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();  // Reset score in ScoreManager
        }
    }

    // Call this method at the end of the game, or when you want to check for a new high score
    public void CheckHighScore()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.CheckAndUpdateHighScore();  // Check if high score needs updating
        }
    }
}
