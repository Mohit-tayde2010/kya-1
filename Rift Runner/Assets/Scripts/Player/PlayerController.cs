using UnityEngine;

/// <summary>
/// Core physics-based 2D player controller with tight, precise movement.
/// Features: Coyote time, jump buffering, variable jump height, mobile/keyboard input.
/// Designed for precision platformers (Celeste/Super Meat Boy style).
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float groundDrag = 8f;
    [SerializeField] private float airDrag = 3f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravityScale = 2.5f;
    [SerializeField] private float maxFallSpeed = 20f;

    [Header("Juice Mechanics")]
    [SerializeField] private float coyoteTime = 0.1f;  // Time after leaving ground to still jump
    [SerializeField] private float jumpBufferTime = 0.1f;  // Time before landing to input jump
    [SerializeField] private float jumpCutOffMultiplier = 0.5f;  // Reduce upward velocity when releasing jump

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckOffset = Vector2.zero;

    // Internal state
    private Rigidbody2D rb;
    private float horizontalInput;
    private float timeSinceGrounded;
    private float timeSinceJumpPressed;
    private bool isJumping;
    private bool wasJumpPressed;

    // Mobile input
    private float mobileHorizontalInput;

    private void OnEnable()
    {
        // Reset input when enabled
        horizontalInput = 0f;
        mobileHorizontalInput = 0f;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("PlayerController requires a Rigidbody2D component!");

        // Set gravity scale
        rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        // Keyboard input (for editor testing)
        horizontalInput = Input.GetAxis("Horizontal");

        // Mobile input (set from UI buttons)
        horizontalInput += mobileHorizontalInput;
        horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);

        // Jump input detection
        if (Input.GetKeyDown(KeyCode.Space))
        {
            timeSinceJumpPressed = 0f;
        }

        // Update timers
        timeSinceGrounded += Time.deltaTime;
        timeSinceJumpPressed += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // Check if grounded
        bool isGrounded = IsGrounded();
        if (isGrounded)
        {
            timeSinceGrounded = 0f;
        }

        // Apply horizontal movement
        ApplyHorizontalMovement();

        // Handle jumping
        HandleJumping(isGrounded);

        // Apply drag based on ground state
        float currentDrag = isGrounded ? groundDrag : airDrag;
        rb.drag = currentDrag;

        // Clamp fall speed
        if (rb.velocity.y < -maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
        }
    }

    /// <summary>
    /// Apply horizontal movement with acceleration/deceleration.
    /// </summary>
    private void ApplyHorizontalMovement()
    {
        if (horizontalInput != 0f)
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }
        else
        {
            // Smooth deceleration
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0f, Time.fixedDeltaTime * groundDrag), rb.velocity.y);
        }
    }

    /// <summary>
    /// Handle jump input with coyote time, jump buffering, and variable height.
    /// </summary>
    private void HandleJumping(bool isGrounded)
    {
        bool canJump = (timeSinceGrounded <= coyoteTime);
        bool jumpPressed = (timeSinceJumpPressed <= jumpBufferTime);

        if (jumpPressed && canJump && !isJumping)
        {
            // Execute jump
            rb.velocity = new Vector2(rb.velocity.x, 0f);  // Reset vertical velocity
            rb.velocity += Vector2.up * jumpForce;
            isJumping = true;
            timeSinceJumpPressed = jumpBufferTime + 0.1f;  // Consume input
        }

        // Release jump early = shorter jump
        if (isJumping && Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutOffMultiplier);
            isJumping = false;
        }

        // Stop jumping if grounded
        if (isGrounded && rb.velocity.y <= 0f)
        {
            isJumping = false;
        }
    }

    /// <summary>
    /// Detect if player is touching ground using Physics2D.OverlapCircle.
    /// </summary>
    private bool IsGrounded()
    {
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        Collider2D hit = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
        return hit != null;
    }

    /// <summary>
    /// Called by mobile UI to set horizontal input.
    /// </summary>
    public void SetMobileHorizontalInput(float value)
    {
        mobileHorizontalInput = value;
    }

    /// <summary>
    /// Called by mobile UI jump button.
    /// </summary>
    public void OnMobileJumpButtonPressed()
    {
        timeSinceJumpPressed = 0f;
    }

    /// <summary>
    /// Called by mobile UI jump button release.
    /// </summary>
    public void OnMobileJumpButtonReleased()
    {
        if (isJumping && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutOffMultiplier);
            isJumping = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check in editor
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
    }
}
