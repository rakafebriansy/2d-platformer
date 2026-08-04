using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    [Header("Collision Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Movement Settings")]   
    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float wallSlideSpeed = 1f;
    [SerializeField] private float wallJumpDuration = 0.25f;
    [SerializeField] private float wallJumpSpeedMultiplier = 1.2f;
    [SerializeField] private float wallJumpForceMultiplier = 0.9f;
    [SerializeField] private float defaultGravityScale = 2.5f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Coyote Time Settings")]   
    [SerializeField] private float coyoteTime = 0.2f;

    [Header("Multiple Jumps Settings")]   
    [SerializeField] private int maxJumps = 2;

    [Header("SFX")]
    [SerializeField] private AudioClip jumpSound;

    private float coyoteTimeCounter;
    private float moveInput = 0f;
    private bool jumpRequested = false;
    private bool jumpCanceled = false;
    private bool wallJumpRequested = false;
    private float wallJumpCooldown;
    private int wallSide;
    private int jumpCount;

    public bool IsGrounded { get; private set; }
    public bool OnWall { get; private set; }

    private static readonly int IsRunningHash = Animator.StringToHash("isRunning");
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int JumpHash = Animator.StringToHash("jump");

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckEnvironment();
        HandleTimers();
        GatherInput();
        HandleVisuals();
    }

    private void FixedUpdate()
    {
        HandleMovement();

        if (jumpRequested)
        {
            ExecuteJump();
            jumpCount++;
        }
        else if (wallJumpRequested)
        {
            ExecuteWallJump();
        }

        if (jumpCanceled)
        {
            if (body.linearVelocity.y > 0f)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y * jumpCutMultiplier);
            }
            jumpCanceled = false;
        }
    }
    
    public bool CanAttack()
    {
        return moveInput == 0 && IsGrounded && !OnWall;
    }

    private void CheckEnvironment()
    {
        Vector2 bottomPos = new (boxCollider.bounds.center.x, boxCollider.bounds.min.y);
        Vector2 rightPos = new (boxCollider.bounds.max.x, boxCollider.bounds.center.y);
        Vector2 leftPos = new (boxCollider.bounds.min.x, boxCollider.bounds.center.y);

        Vector2 groundSensorSize = new (boxCollider.bounds.size.x - 0.05f, 0.1f);
        Vector2 wallSensorSize = new (0.1f, boxCollider.bounds.size.y * 0.8f);

        IsGrounded = Physics2D.OverlapBox(bottomPos, groundSensorSize, 0, groundLayer) != null;
        bool wallOnRight = Physics2D.OverlapBox(rightPos, wallSensorSize, 0, wallLayer) != null;
        bool wallOnLeft = Physics2D.OverlapBox(leftPos, wallSensorSize, 0, wallLayer) != null;

        OnWall = wallOnRight || wallOnLeft;

        if (wallOnRight) wallSide = 1;
        else if (wallOnLeft) wallSide = -1;
        else wallSide = 0;
    }

    private void ExecuteJump()
    {
        if (jumpSound != null && SoundManager.instance != null)
        {
            SoundManager.instance.PlaySound(jumpSound);
        }
        anim.SetTrigger(JumpHash);
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        jumpRequested = false;
    }

    private void ExecuteWallJump()
    {
        if (jumpSound != null && SoundManager.instance != null)
        {
            SoundManager.instance.PlaySound(jumpSound);
        }
        float jumpDirection = -wallSide; 
        wallJumpCooldown = wallJumpDuration;
        
        body.linearVelocity = new Vector2(jumpDirection * speed * wallJumpSpeedMultiplier, jumpForce * wallJumpForceMultiplier);
        
        transform.localScale = new Vector3(jumpDirection, 1f, 1f);
        anim.SetTrigger(JumpHash);
        wallJumpRequested = false;
    }

    private void GatherInput()
    {
        moveInput = 0f;
        if (Keyboard.current == null) return;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) 
            moveInput = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) 
            moveInput = 1f;

        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            if (OnWall && !IsGrounded)
                wallJumpRequested = true;
            else if (coyoteTimeCounter > 0f)
            {
                jumpRequested = true;
                coyoteTimeCounter = 0f;
            } else if (jumpCount < maxJumps)
            {
                if (jumpCount == 0) jumpCount = 1;
                jumpRequested = true;
            }
        }

        if (Keyboard.current.spaceKey.wasReleasedThisFrame || Keyboard.current.upArrowKey.wasReleasedThisFrame)
        {
            jumpCanceled = true;
        }
    }

    private void HandleTimers()
    {
        if (IsGrounded)
            wallJumpCooldown = 0f;
        else if (wallJumpCooldown > 0f)
            wallJumpCooldown -= Time.deltaTime;

        if (IsGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            jumpCount = 0;
        }
        else if (OnWall)
            jumpCount = 0;
        else
            coyoteTimeCounter -= Time.deltaTime;
    }

    private void HandleMovement()
    {
        bool pressingIntoWall = (wallSide == 1 && moveInput > 0f) || (wallSide == -1 && moveInput < 0f);

        if (OnWall && !IsGrounded && pressingIntoWall && body.linearVelocity.y <= 0f)
        {
            wallJumpCooldown = 0f;

            body.gravityScale = 0f;

            body.linearVelocity = new Vector2(moveInput * speed, -wallSlideSpeed);
            return;
        }

        if (wallJumpCooldown > 0f)
        {
            body.gravityScale = defaultGravityScale;

            if (Mathf.Abs(moveInput) > 0.01f)
            {
                body.linearVelocity = new Vector2(moveInput * speed, body.linearVelocity.y);
            }
            return;
        }

        body.gravityScale = defaultGravityScale;
        body.linearVelocity = new Vector2(moveInput * speed, body.linearVelocity.y);
    }

    private void HandleVisuals()
    {
        if (wallJumpCooldown <= 0f && !OnWall)
        {
            if (moveInput > 0.01f) transform.localScale = Vector3.one;
            else if (moveInput < -0.01f) transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (OnWall && !IsGrounded)
        {
            transform.localScale = new Vector3(wallSide, 1f, 1f);
        }

        bool isTryingToMove = Mathf.Abs(moveInput) > 0.01f;
        anim.SetBool(IsRunningHash, isTryingToMove && IsGrounded);
        
        anim.SetBool(IsGroundedHash, IsGrounded);
    }


}
