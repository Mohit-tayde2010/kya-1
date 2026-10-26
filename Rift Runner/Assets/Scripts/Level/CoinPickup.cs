using UnityEngine;

/// <summary>
/// Handles coin/collectible pickup logic.
/// On trigger enter with player, collect the coin and destroy the object.
/// </summary>
public class CoinPickup : MonoBehaviour
{
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private float destroyDelay = 0.1f;

    private bool alreadyCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if colliding with player
        if (collision.CompareTag("Player") && !alreadyCollected)
        {
            alreadyCollected = true;
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        // Play sound
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Notify level manager
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.CollectCoin();
        }

        // Destroy coin after brief delay
        Destroy(gameObject, destroyDelay);
    }
}
