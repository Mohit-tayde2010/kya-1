using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles mobile on-screen button inputs.
/// Manages left, right, and jump button events.
/// </summary>
public class MobileInputHandler : MonoBehaviour
{
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button jumpButton;

    private PlayerController playerController;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("MobileInputHandler: PlayerController not found!");
            return;
        }

        // Register button callbacks
        SetupButton(leftButton, () => OnLeftPressed(), () => OnLeftReleased());
        SetupButton(rightButton, () => OnRightPressed(), () => OnRightReleased());
        SetupButton(jumpButton, () => OnJumpPressed(), () => OnJumpReleased());
    }

    private void SetupButton(Button button, System.Action onPress, System.Action onRelease)
    {
        if (button == null)
        {
            Debug.LogWarning("Mobile button not assigned!");
            return;
        }

        // Add event trigger for press and release
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // Pointer Down event
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        pointerDownEntry.callback.AddListener((data) => onPress());
        trigger.triggers.Add(pointerDownEntry);

        // Pointer Up event
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        pointerUpEntry.callback.AddListener((data) => onRelease());
        trigger.triggers.Add(pointerUpEntry);
    }

    private void OnLeftPressed()
    {
        if (playerController != null)
            playerController.SetMobileHorizontalInput(-1f);
    }

    private void OnLeftReleased()
    {
        if (playerController != null)
            playerController.SetMobileHorizontalInput(0f);
    }

    private void OnRightPressed()
    {
        if (playerController != null)
            playerController.SetMobileHorizontalInput(1f);
    }

    private void OnRightReleased()
    {
        if (playerController != null)
            playerController.SetMobileHorizontalInput(0f);
    }

    private void OnJumpPressed()
    {
        if (playerController != null)
            playerController.OnMobileJumpButtonPressed();
    }

    private void OnJumpReleased()
    {
        if (playerController != null)
            playerController.OnMobileJumpButtonReleased();
    }
}

/// <summary>
/// Placeholder for Button type used in mobile input handler.
/// If this conflicts with UnityEngine.UI.Button, remove this or adjust imports.
/// </summary>
public class Button { }
