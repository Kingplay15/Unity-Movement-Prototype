using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    enum MovementMode
    {
        Arcade,
        Platformer,
        Physics
    }

    [SerializeField] MovementMode mode = MovementMode.Arcade;

    Rigidbody2D rb;
    float moveInput = 0f;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float accelRate = 10f; //For acceleration
    [SerializeField] float linearDrag = 1f; //For deceleration

    [SerializeField] float jumpForce = 2f;
    bool jumpRequested = false;
    bool jumpReleased = false;
    bool isGrounded = false;

    [SerializeField] float fallGravity = 2f;
    float baseGravity;

    //Raycast variables
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] float groundCheckRadius = 0.1f;

    [SerializeField] float coyoteTime = 0.2f; // Time after leaving ground where jump is still allowed
    float coyoteTimeCounter = 0f;
    [SerializeField] float jumpBufferTime = 0.1f; // Time before landing where jump input is buffered
    float jumpBufferCounter = 0f;

    [SerializeField] UIManagement UIManager;


    void Awake()
    {
        mode = MovementMode.Arcade;
        rb = GetComponent<Rigidbody2D>();
        baseGravity = rb.gravityScale;
        rb.linearDamping = linearDrag;
    }

    void Start()
    {
        UIManager.UpdateMode((int)mode + 1);
    }

    void Update()
    {
        CheckMoveInput();
        CheckSwitchModeInput();

        // Record a jump request (transient press)
        if (!jumpRequested && Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpRequested = true;

        // Set / decay the jump buffer based on the raw input event (wasPressedThisFrame).
        JumpBuffer();

        // Record a jump release (transient release)
        if (!isGrounded && Keyboard.current.spaceKey.wasReleasedThisFrame)
            jumpReleased = true;
    }

    void CheckMoveInput()
    {
        moveInput = 0f;
        if (Keyboard.current.aKey.isPressed)
            moveInput = -1f;
        if (Keyboard.current.dKey.isPressed)
            moveInput = 1f;
    }

    void CheckSwitchModeInput()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            mode = MovementMode.Arcade;
            rb.linearVelocity = Vector2.zero; // Reset velocity when switching modes to prevent carryover
            UIManager.UpdateMode(1);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            mode = MovementMode.Platformer;
            rb.linearVelocity = Vector2.zero;
            UIManager.UpdateMode(2);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            mode = MovementMode.Physics;
            rb.linearVelocity = Vector2.zero;
            UIManager.UpdateMode(3);
        }
    }

    void Coyote()
    {
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.fixedDeltaTime;

        if (coyoteTimeCounter < 0f)
            coyoteTimeCounter = 0f;
    }

    void JumpBuffer()
    {
        // Capture the press event once and then let it decay over time.
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpBufferCounter = jumpBufferTime;

        // decay the buffer (Update uses Time.deltaTime)
        jumpBufferCounter -= Time.deltaTime;
        if (jumpBufferCounter < 0f)
            jumpBufferCounter = 0f;
    }

    void FixedUpdate()
    {
        IsGrounded();

        if (mode == MovementMode.Platformer)
            Coyote();

        Move();
        HandleJump();
        HandleGravity();
    }

    void Move()
    {
        switch (mode)
        {
            case MovementMode.Arcade:
                ArcadeMove();
                break;

            case MovementMode.Platformer:
                PlatformerMove();
                break;

            case MovementMode.Physics:
                PhysicsMove();
                break;
        }
    }

    void ArcadeMove()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocityY);
    }

    void PlatformerMove()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocityY);
    }

    void PhysicsMove()
    {
        rb.AddForce(Vector2.right * moveInput * accelRate);

        //Clamp the velocity to prevent exceeding max speed only
        if (Mathf.Abs(rb.linearVelocityX) > moveSpeed)
        {
            float clampedVelocityX = Mathf.Clamp(rb.linearVelocityX, -moveSpeed, moveSpeed);
            rb.linearVelocity = new Vector2(clampedVelocityX, rb.linearVelocityY);
        }
    }

    void HandleJump()
    {
        switch (mode)
        {
            case MovementMode.Arcade:
                SimpleJump();
                break;

            case MovementMode.Platformer:
                AdvancedJump();
                break;

            case MovementMode.Physics:
                PhysicsJump();
                break;
        }
    }

    void SimpleJump()
    {
        if (jumpRequested)
        {
            if (isGrounded)
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            jumpRequested = false; // Reset jump request after processing
        }
    }

    void AdvancedJump()
    {
        // Jump trigger
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);

            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
        }

        // Variable jump (cut jump)
        if (jumpReleased && rb.linearVelocityY > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);
        }

        jumpRequested = false; // Reset jump request (for the other 2 modes) after processing
        jumpReleased = false;
    }

    void PhysicsJump()
    {
        if (jumpRequested)
        {
            if (isGrounded)
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            jumpRequested = false; // Reset jump request after processing
        }
    }

    void HandleGravity()
    {
        switch (mode)
        {
            case MovementMode.Platformer:
                BetterFalling();
                break;

            case MovementMode.Arcade:
            case MovementMode.Physics:
                rb.gravityScale = baseGravity;
                break;
        }
    }

    void BetterFalling()
    {
        if (rb.linearVelocityY < 0f) //Falling
            rb.gravityScale = fallGravity;
        else rb.gravityScale = baseGravity;
    }

    void IsGrounded()
    {
        // Use BoxCast with correct parameters: origin, size, angle, direction, distance, layerMask
        Vector2 origin = transform.position;
        Vector2 size = Vector2.one * groundCheckRadius; // You may want to adjust this size to match your collider
        float angle = 0f;
        Vector2 direction = Vector2.down;
        float distance = groundCheckDistance;
        int layerMask = groundLayer.value;

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, angle, direction, distance, layerMask);
        isGrounded = hit.collider != null;
    }

    void OnDrawGizmosSelected()
    {
        // Visualize the BoxCast used in IsGrounded
        Vector2 origin = transform.position;
        Vector2 size = Vector2.one * groundCheckRadius; // match IsGrounded size
        Vector2 direction = Vector2.down;
        float distance = groundCheckDistance;

        Vector3 start = origin;
        Vector3 end = start + (Vector3)direction * distance;
        Vector3 size3 = new Vector3(size.x, size.y, 0.01f);

        // Draw start and end boxes and a line connecting their centers
        Gizmos.color = isGrounded ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(start, size3);
        Gizmos.DrawWireCube(end, size3);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(start, end);
    }
}
