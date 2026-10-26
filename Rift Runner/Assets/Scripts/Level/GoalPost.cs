using UnityEngine;

/// <summary>
/// Goal post trigger that completes the level when player touches it.
/// </summary>
public class GoalPost : MonoBehaviour
{
    private bool alreadyTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !alreadyTriggered)
        {
            alreadyTriggered = true;

            LevelManager levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null)
            {
                levelManager.CompleteLevel();
            }
        }
    }
}
