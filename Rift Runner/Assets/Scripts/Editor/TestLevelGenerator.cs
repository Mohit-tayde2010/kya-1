using UnityEngine;

/// <summary>
/// Template for quickly generating a test level layout.
/// Attach to empty GameObject and call GenerateTestLevel() in editor.
/// </summary>
public class TestLevelGenerator : MonoBehaviour
{
    [SerializeField] private Sprite platformSprite;
    [SerializeField] private Sprite coinSprite;
    [SerializeField] private Sprite spikeSprite;
    [SerializeField] private Sprite playerSprite;

    [SerializeField] private Material platformMaterial;
    [SerializeField] private LayerMask groundLayer;

    public void GenerateTestLevel()
    {
        if (platformSprite == null)
        {
            Debug.LogError("Platform sprite not assigned!");
            return;
        }

        // Clear any existing objects (optional)
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // Ground platform
        CreatePlatform("Ground", Vector3.zero, new Vector2(20, 2), 0);

        // Platform staircase
        CreatePlatform("Platform_1", new Vector3(3, 3, 0), new Vector2(3, 1), 0);
        CreatePlatform("Platform_2", new Vector3(6, 5, 0), new Vector2(3, 1), 0);
        CreatePlatform("Platform_3", new Vector3(9, 7, 0), new Vector2(3, 1), 0);

        // Gap with spikes
        CreateSpike("Spike_1", new Vector3(12, 6, 0));
        CreateSpike("Spike_2", new Vector3(13, 6, 0));

        // Landing platform
        CreatePlatform("Platform_4", new Vector3(15, 5, 0), new Vector2(3, 1), 0);

        // Coins on stairs
        CreateCoin("Coin_1", new Vector3(3, 4.5f, 0));
        CreateCoin("Coin_2", new Vector3(6, 6.5f, 0));
        CreateCoin("Coin_3", new Vector3(9, 8.5f, 0));
        CreateCoin("Coin_4", new Vector3(15, 6.5f, 0));

        // Goal
        CreateGoal("Goal", new Vector3(18, 6, 0));

        Debug.Log("Test level generated! Don't forget to:\n1. Create player and add to scene\n2. Create LevelManager GameObject\n3. Create Canvas with buttons");
    }

    private void CreatePlatform(string name, Vector3 position, Vector2 size, float rotation)
    {
        GameObject platform = new GameObject(name);
        platform.transform.SetParent(transform);
        platform.transform.position = position;
        platform.layer = LayerMask.NameToLayer("Ground");

        SpriteRenderer sr = platform.AddComponent<SpriteRenderer>();
        sr.sprite = platformSprite;
        sr.sortingOrder = 0;

        BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
        collider.size = size;
        collider.isTrigger = false;

        platform.transform.localScale = size;
    }

    private void CreateCoin(string name, Vector3 position)
    {
        GameObject coin = new GameObject(name);
        coin.transform.SetParent(transform);
        coin.transform.position = position;
        coin.layer = LayerMask.NameToLayer("Collectible");

        SpriteRenderer sr = coin.AddComponent<SpriteRenderer>();
        sr.sprite = coinSprite;
        sr.sortingOrder = 1;

        CircleCollider2D collider = coin.AddComponent<CircleCollider2D>();
        collider.radius = 0.3f;
        collider.isTrigger = true;

        CoinPickup pickup = coin.AddComponent<CoinPickup>();
    }

    private void CreateSpike(string name, Vector3 position)
    {
        GameObject spike = new GameObject(name);
        spike.transform.SetParent(transform);
        spike.transform.position = position;
        spike.layer = LayerMask.NameToLayer("Hazard");

        SpriteRenderer sr = spike.AddComponent<SpriteRenderer>();
        sr.sprite = spikeSprite;
        sr.sortingOrder = 0;

        BoxCollider2D collider = spike.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1, 1);
        collider.isTrigger = true;

        Hazard hazard = spike.AddComponent<Hazard>();
    }

    private void CreateGoal(string name, Vector3 position)
    {
        GameObject goal = new GameObject(name);
        goal.transform.SetParent(transform);
        goal.transform.position = position;

        SpriteRenderer sr = goal.AddComponent<SpriteRenderer>();
        sr.sprite = platformSprite;
        sr.color = new Color(1, 0.84f, 0, 1); // Gold color
        sr.sortingOrder = 0;

        BoxCollider2D collider = goal.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(2, 2);
        collider.isTrigger = true;

        GoalPost goalPost = goal.AddComponent<GoalPost>();
    }
}
