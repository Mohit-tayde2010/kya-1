using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the win screen with star rewards, time, and coins.
/// Provides buttons to restart level or go to next level.
/// </summary>
public class WinScreen : MonoBehaviour
{
    [SerializeField] private Text starsText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text coinsText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button restartLevelButton;

    [SerializeField] private Image[] starImages;  // Visual star indicators
    [SerializeField] private Color earnedStarColor = Color.yellow;
    [SerializeField] private Color unearnedStarColor = Color.gray;

    public void Setup(int starsEarned, float elapsedTime, int coinsCollected, int totalCoins)
    {
        // Display text
        starsText.text = $"Stars: {starsEarned}/3";
        timeText.text = $"Time: {elapsedTime:F2}s";
        coinsText.text = $"Coins: {coinsCollected}/{totalCoins}";

        // Update star visuals
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].color = (i < starsEarned) ? earnedStarColor : unearnedStarColor;
        }

        // Setup buttons
        nextLevelButton.onClick.AddListener(OnNextLevelPressed);
        restartLevelButton.onClick.AddListener(OnRestartPressed);

        // Pause game
        Time.timeScale = 0f;
    }

    private void OnNextLevelPressed()
    {
        Time.timeScale = 1f;
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.NextLevel();
        }
    }

    private void OnRestartPressed()
    {
        Time.timeScale = 1f;
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.RestartLevel();
        }
    }
}
