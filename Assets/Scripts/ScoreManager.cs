using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int score { get; private set; }

    public TMP_Text scoreText;     // TMP text component to display score
    public TMP_Text highScoreText; // TMP text component to display high score

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

    private void Start()
    {
        UpdateScoreText();  // Initialize the score and high score display
        PlayerPrefs.DeleteKey("HighScore"); // Remove the high score from PlayerPrefs
        UpdateScoreText();
    }

    // Call this method when a balloon is popped
    public void AddScore(int points)
    {
        score += points;          // Add points to the current score
        UpdateScoreText();        // Update the score on the UI
        CheckAndUpdateHighScore(); // Check and update the high score if needed
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();  // Reset score display
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + (score / 10).ToString();  // Update score text
        }

        // Update high score from PlayerPrefs
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + (highScore / 10).ToString();  // Update high score text
        }
    }

    public void CheckAndUpdateHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            // Save the new high score in PlayerPrefs
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();  // Ensure PlayerPrefs is saved immediately

            // Update the high score display
            highScoreText.text = "High Score: " + (score / 10).ToString();
        }
    }
}
