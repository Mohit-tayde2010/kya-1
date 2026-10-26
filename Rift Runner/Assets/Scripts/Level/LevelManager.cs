using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages a single level: timer, collectibles, stars, and win conditions.
/// Tracks 3-star performance and calculates rewards.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Level Configuration")]
    [SerializeField] private int levelID = 1;
    [SerializeField] private float targetTime = 60f;  // Time limit for 2-star
    [SerializeField] private int totalCoinsInLevel = 10;

    [Header("UI References")]
    [SerializeField] private GameObject winScreenPrefab;
    [SerializeField] private Canvas uiCanvas;

    // Internal state
    private float elapsedTime;
    private int coinsCollected;
    private bool isLevelComplete;
    private bool isTimerRunning;

    // Events for UI
    public delegate void OnCoinCollected(int totalCoins);
    public static event OnCoinCollected CoinCollected;

    public delegate void OnLevelComplete(int starsEarned, float timeUsed, int coinsCollected);
    public static event OnLevelComplete LevelComplete;

    private void Start()
    {
        elapsedTime = 0f;
        coinsCollected = 0;
        isLevelComplete = false;
        isTimerRunning = true;
    }

    private void Update()
    {
        if (isTimerRunning && !isLevelComplete)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Register a coin collection.
    /// </summary>
    public void CollectCoin()
    {
        coinsCollected++;
        CoinCollected?.Invoke(coinsCollected);
    }

    /// <summary>
    /// Called when player reaches the goal.
    /// </summary>
    public void CompleteLevel()
    {
        if (isLevelComplete)
            return;

        isLevelComplete = true;
        isTimerRunning = false;

        // Calculate stars
        int starsEarned = CalculateStars();

        // Update global progression
        GameManager.Instance.UpdateLevelProgress(levelID, starsEarned, coinsCollected);

        // Notify listeners
        LevelComplete?.Invoke(starsEarned, elapsedTime, coinsCollected);

        // Show win screen
        ShowWinScreen(starsEarned);
    }

    /// <summary>
    /// Calculate stars based on performance.
    /// Star 1: Level completed.
    /// Star 2: Time is below target.
    /// Star 3: All coins collected.
    /// </summary>
    private int CalculateStars()
    {
        int stars = 1;  // Star 1: Completion

        if (elapsedTime < targetTime)
            stars++;  // Star 2: Speed

        if (coinsCollected == totalCoinsInLevel)
            stars++;  // Star 3: Collection

        return stars;
    }

    /// <summary>
    /// Display the win screen with earned stars.
    /// </summary>
    private void ShowWinScreen(int starsEarned)
    {
        if (winScreenPrefab == null)
        {
            Debug.LogWarning("Win screen prefab not assigned to LevelManager!");
            return;
        }

        GameObject winScreen = Instantiate(winScreenPrefab, uiCanvas.transform);
        WinScreen winScreenComponent = winScreen.GetComponent<WinScreen>();
        if (winScreenComponent != null)
        {
            winScreenComponent.Setup(starsEarned, elapsedTime, coinsCollected, totalCoinsInLevel);
        }
    }

    /// <summary>
    /// Restart the current level.
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Go to next level.
    /// </summary>
    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Getters
    public float GetElapsedTime() => elapsedTime;
    public int GetCoinsCollected() => coinsCollected;
    public int GetTotalCoins() => totalCoinsInLevel;
    public float GetTargetTime() => targetTime;
    public bool IsLevelComplete() => isLevelComplete;
}
