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

    [SerializeField] float jumpForce = 2f;
    //bool jumpRequested = false;
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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseGravity = rb.gravityScale;
    }

    void Update()
    {
        moveInput = 0f;
        if (Keyboard.current.aKey.isPressed)
            moveInput = -1f;
        if (Keyboard.current.dKey.isPressed)
            moveInput = 1f;

        // Record a jump request (transient press)
        //if (!jumpRequested && Keyboard.current.spaceKey.wasPressedThisFrame)
        //    jumpRequested = true;

        // Set / decay the jump buffer based on the raw input event (wasPressedThisFrame).
        JumpBuffer();

        if (!isGrounded && Keyboard.current.spaceKey.wasReleasedThisFrame)
            jumpReleased = true;

        Debug.Log(isGrounded ? "Grounded" : "Airborne");
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
        // Physics-grounded check and coyote timing should run in FixedUpdate for correct timing
        IsGrounded();
        Coyote();

        Move();

        // Use the buffered input or a coyote + instant-input jump
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
            Jump();

        VariableJumpHeight();
        BetterFalling();
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
        }
    }

    void ArcadeMove()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocityY);
    }

    void PlatformerMove()
    {
        float targetSpeed = moveInput * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocityX;
        rb.AddForce(Vector2.right * speedDiff * accelRate);

        //Clamp the velocity to the max speed
        float newVelocityX = Mathf.Clamp(rb.linearVelocityX, -moveSpeed, moveSpeed);
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocityY);
        Debug.Log(speedDiff);
    }

    void Jump()
    {
        //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        //jumpRequested = false;
        coyoteTimeCounter = 0f; // Reset coyote time on jump
        jumpBufferCounter = 0f; // Clear jump buffer on jump

        Debug.Log("Jump!");
    }

    void VariableJumpHeight()
    {
        if (jumpReleased && rb.linearVelocity.y > 0)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        jumpReleased = false;
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
