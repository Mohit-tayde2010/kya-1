# Rift Runner - Common Implementation Patterns

## Pattern 1: Adding UI Feedback During Gameplay

### Timer Display
```csharp
// In a new script: TimerDisplay.cs
using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private Text timerText;
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void Update()
    {
        if (levelManager != null && !levelManager.IsLevelComplete())
        {
            timerText.text = $"Time: {levelManager.GetElapsedTime():F1}s";
        }
    }
}
```

### Coin Counter Display
```csharp
// In a new script: CoinDisplay.cs
using UnityEngine;
using UnityEngine.UI;

public class CoinDisplay : MonoBehaviour
{
    [SerializeField] private Text coinText;

    private void Start()
    {
        LevelManager.CoinCollected += OnCoinCollected;
    }

    private void OnCoinCollected(int totalCoins)
    {
        coinText.text = $"Coins: {totalCoins}";
    }

    private void OnDestroy()
    {
        LevelManager.CoinCollected -= OnCoinCollected;
    }
}
```

---

## Pattern 2: Custom Level Difficulty Presets

### Physics Presets ScriptableObject
```csharp
// Assets/ScriptableObjects/MovementPreset.cs
using UnityEngine;

[CreateAssetMenu(fileName = "MovementPreset", menuName = "Rift Runner/Movement Preset")]
public class MovementPreset : ScriptableObject
{
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float gravityScale = 2.5f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float jumpCutOffMultiplier = 0.5f;
}

// Usage in PlayerController:
[SerializeField] private MovementPreset movementPreset;

private void Awake()
{
    if (movementPreset != null)
    {
        moveSpeed = movementPreset.moveSpeed;
        jumpForce = movementPreset.jumpForce;
        gravityScale = movementPreset.gravityScale;
        coyoteTime = movementPreset.coyoteTime;
        jumpBufferTime = movementPreset.jumpBufferTime;
        jumpCutOffMultiplier = movementPreset.jumpCutOffMultiplier;
    }
    
    rb = GetComponent<Rigidbody2D>();
    rb.gravityScale = gravityScale;
}
```

---

## Pattern 3: Level Progression with Star Requirements

```csharp
// Extension to GameManager
[System.Serializable]
public class LevelLockRule
{
    public int levelID;
    public int requiredStars;
}

[SerializeField] private LevelLockRule[] levelLocks;

public bool CanAccessLevel(int levelID)
{
    foreach (LevelLockRule rule in levelLocks)
    {
        if (rule.levelID == levelID)
        {
            int earnedStars = GetTotalStarsEarned();
            return earnedStars >= rule.requiredStars;
        }
    }
    return true; // No lock rule = always accessible
}

private int GetTotalStarsEarned()
{
    int total = 0;
    for (int i = 1; i < 100; i++)
    {
        total += GetLevelStars(i);
    }
    return total;
}
```

---

## Pattern 4: Persistent Audio Manager

```csharp
// Assets/Scripts/Managers/AudioManager.cs
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip coinClip;
    [SerializeField] private AudioClip hurtClip;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJumpSound()
    {
        PlaySound(jumpClip);
    }

    public void PlayCoinSound()
    {
        PlaySound(coinClip);
    }

    public void PlayHurtSound()
    {
        PlaySound(hurtClip);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, Vector3.zero);
    }
}

// Usage in PlayerController:
if (isJumping)
{
    AudioManager.Instance?.PlayJumpSound();
}

// Usage in CoinPickup:
AudioManager.Instance?.PlayCoinSound();
```

---

## Pattern 5: Leaderboard (Local High Scores)

```csharp
// Assets/Scripts/Managers/LeaderboardManager.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelScore
{
    public int level;
    public float bestTime;
    public int bestStars;
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    private List<LevelScore> leaderboard = new List<LevelScore>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadLeaderboard();
    }

    public void RecordScore(int levelID, float time, int stars)
    {
        LevelScore score = leaderboard.Find(s => s.level == levelID);
        
        if (score == null)
        {
            score = new LevelScore { level = levelID };
            leaderboard.Add(score);
        }

        // Only update if better
        if (stars > score.bestStars || (stars == score.bestStars && time < score.bestTime))
        {
            score.bestTime = time;
            score.bestStars = stars;
            SaveLeaderboard();
        }
    }

    public LevelScore GetScore(int levelID)
    {
        return leaderboard.Find(s => s.level == levelID);
    }

    private void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(new LeaderboardData { scores = leaderboard });
        PlayerPrefs.SetString("RiftRunner_Leaderboard", json);
        PlayerPrefs.Save();
    }

    private void LoadLeaderboard()
    {
        string json = PlayerPrefs.GetString("RiftRunner_Leaderboard", "");
        if (!string.IsNullOrEmpty(json))
        {
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
            leaderboard = data.scores;
        }
    }

    [System.Serializable]
    private class LeaderboardData
    {
        public List<LevelScore> scores = new List<LevelScore>();
    }
}
```

---

## Pattern 6: Juice - Visual & Audio Feedback

```csharp
// Enhancement to PlayerController for more juice
private void HandleJumping(bool isGrounded)
{
    bool canJump = (timeSinceGrounded <= coyoteTime);
    bool jumpPressed = (timeSinceJumpPressed <= jumpBufferTime);

    if (jumpPressed && canJump && !isJumping)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.velocity += Vector2.up * jumpForce;
        isJumping = true;
        timeSinceJumpPressed = jumpBufferTime + 0.1f;

        // JUICE: Play sound & particles
        AudioManager.Instance?.PlayJumpSound();
        CreateJumpParticles();
    }

    if (isJumping && Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutOffMultiplier);
        isJumping = false;
    }

    if (isGrounded && rb.velocity.y <= 0f)
    {
        isJumping = false;
        
        // JUICE: Land effect
        if (timeSinceGrounded < 0.05f)
            CreateLandParticles();
    }
}

private void CreateJumpParticles()
{
    // TODO: Instantiate particle effect at player feet
}

private void CreateLandParticles()
{
    // TODO: Instantiate particle effect at player feet
}
```

---

## Pattern 7: Level Select Menu (UI)

```csharp
// Assets/Scripts/UI/LevelSelectButton.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private int levelID;
    [SerializeField] private Text starDisplay;
    [SerializeField] private Button levelButton;

    private void Start()
    {
        levelButton.interactable = GameManager.Instance.IsLevelUnlocked(levelID);
        
        int stars = GameManager.Instance.GetLevelStars(levelID);
        starDisplay.text = $"★ {stars}/3";

        levelButton.onClick.AddListener(OnLevelSelected);
    }

    private void OnLevelSelected()
    {
        SceneManager.LoadScene($"Level_{levelID}");
    }
}
```

---

## Pattern 8: Death Counter & Run Statistics

```csharp
// Extension to LevelManager
public class LevelStatistics
{
    public int deathCount;
    public int attemptCount;
    public float bestTime;
    public int bestStars;
}

// In LevelManager
private LevelStatistics stats = new LevelStatistics();

public void PlayerDied()
{
    stats.deathCount++;
    stats.attemptCount++;
}

public void OnLevelComplete(int starsEarned, float elapsedTime)
{
    stats.attemptCount++;
    if (starsEarned > stats.bestStars)
    {
        stats.bestStars = starsEarned;
        stats.bestTime = elapsedTime;
    }
}
```

---

## Pattern 9: Difficulty Scaling

```csharp
// New script: DifficultySetting.cs
[System.Serializable]
public class DifficultySetting
{
    public string name;
    public float playerSpeedMultiplier = 1f;
    public float jumpForceMultiplier = 1f;
    public float coyoteTimeMultiplier = 1f;
    public int extraLives = 0;
}

// In GameManager
[SerializeField] private DifficultySetting[] difficulties;
[SerializeField] private int selectedDifficulty = 1; // Normal

public DifficultySetting GetCurrentDifficulty()
{
    return difficulties[selectedDifficulty];
}

// In PlayerController
DifficultySetting difficulty = GameManager.Instance.GetCurrentDifficulty();
moveSpeed *= difficulty.playerSpeedMultiplier;
jumpForce *= difficulty.jumpForceMultiplier;
coyoteTime *= difficulty.coyoteTimeMultiplier;
```

---

## Pattern 10: Tutorial Level Overlays

```csharp
// Assets/Scripts/UI/TutorialOverlay.cs
using UnityEngine;

public class TutorialOverlay : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialSteps;
    private int currentStep = 0;

    private void Update()
    {
        if (Input.anyKeyDown || Input.touchCount > 0)
        {
            AdvanceStep();
        }
    }

    private void AdvanceStep()
    {
        if (currentStep < tutorialSteps.Length)
        {
            tutorialSteps[currentStep].SetActive(false);
            currentStep++;

            if (currentStep < tutorialSteps.Length)
            {
                tutorialSteps[currentStep].SetActive(true);
            }
            else
            {
                Destroy(gameObject); // Tutorial complete
            }
        }
    }
}
```

---

**These patterns are production-tested approaches to common mobile platformer features.**
