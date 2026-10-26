using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Generic hazard that kills the player on contact and restarts the level.
/// Attach to spikes, falling platforms, lava, etc.
/// </summary>
public class Hazard : MonoBehaviour
{
    [SerializeField] private float restartDelay = 0.2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            KillPlayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        // Disable player
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Restart level after brief delay
        Invoke(nameof(RestartLevel), restartDelay);
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
