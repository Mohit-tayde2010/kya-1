using UnityEngine;

/// <summary>
/// Global Singleton game manager for persistent progression data.
/// Handles currency, level unlocking, and save/load via PlayerPrefs.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    [Header("Progression")]
    [SerializeField] private int currencyPerStar = 10;
    [SerializeField] private int currencyPerCoin = 5;

    // Runtime data
    private int totalCurrency;

    // Constants for PlayerPrefs
    private const string CURRENCY_KEY = "RiftRunner_Currency";
    private const string STARS_KEY_PREFIX = "RiftRunner_Stars_Level_";
    private const string UNLOCKED_KEY_PREFIX = "RiftRunner_Unlocked_Level_";

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved data
        LoadProgression();
    }

    /// <summary>
    /// Load all saved progression data from PlayerPrefs.
    /// </summary>
    private void LoadProgression()
    {
        totalCurrency = PlayerPrefs.GetInt(CURRENCY_KEY, 0);
        Debug.Log($"Loaded progression. Total Currency: {totalCurrency}");
    }

    /// <summary>
    /// Save all progression data to PlayerPrefs.
    /// </summary>
    private void SaveProgression()
    {
        PlayerPrefs.SetInt(CURRENCY_KEY, totalCurrency);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Update level progress after completion.
    /// Awards currency based on stars and coins collected.
    /// </summary>
    public void UpdateLevelProgress(int levelID, int starsEarned, int coinsCollected)
    {
        // Get previous stars for this level
        int previousStars = PlayerPrefs.GetInt(STARS_KEY_PREFIX + levelID, 0);

        // Only award new stars (don't double-reward)
        int newStars = Mathf.Max(0, starsEarned - previousStars);

        // Award currency
        int currencyReward = (newStars * currencyPerStar) + (coinsCollected * currencyPerCoin);
        totalCurrency += currencyReward;

        Debug.Log($"Level {levelID} Complete! Stars: {starsEarned} | Currency Earned: {currencyReward} | Total Currency: {totalCurrency}");

        // Save progress
        PlayerPrefs.SetInt(STARS_KEY_PREFIX + levelID, starsEarned);
        PlayerPrefs.SetInt(UNLOCKED_KEY_PREFIX + (levelID + 1), 1);  // Unlock next level
        SaveProgression();
    }

    /// <summary>
    /// Check if a level is unlocked.
    /// </summary>
    public bool IsLevelUnlocked(int levelID)
    {
        if (levelID == 1)
            return true;  // First level always unlocked

        return PlayerPrefs.GetInt(UNLOCKED_KEY_PREFIX + levelID, 0) == 1;
    }

    /// <summary>
    /// Get stars earned on a specific level.
    /// </summary>
    public int GetLevelStars(int levelID)
    {
        return PlayerPrefs.GetInt(STARS_KEY_PREFIX + levelID, 0);
    }

    /// <summary>
    /// Spend currency (e.g., for character purchase).
    /// Returns true if successful.
    /// </summary>
    public bool TrySpendCurrency(int amount)
    {
        if (totalCurrency >= amount)
        {
            totalCurrency -= amount;
            SaveProgression();
            return true;
        }

        Debug.LogWarning($"Insufficient currency! Need {amount}, have {totalCurrency}");
        return false;
    }

    /// <summary>
    /// Add currency (for cheats/rewards).
    /// </summary>
    public void AddCurrency(int amount)
    {
        totalCurrency += amount;
        SaveProgression();
    }

    /// <summary>
    /// Reset all progression (for testing).
    /// </summary>
    public void ResetProgression()
    {
        PlayerPrefs.DeleteAll();
        totalCurrency = 0;
        Debug.LogWarning("Progression reset!");
    }

    // Getters
    public int GetTotalCurrency() => totalCurrency;
}
