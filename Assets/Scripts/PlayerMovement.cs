using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    [Header("Collision Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Movement Settings")]   
    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float wallSlideSpeed = 1f;
    [SerializeField] private float wallJumpDuration = 0.25f;

    private float moveInput = 0f;
    private bool jumpRequested = false;
    private bool wallJumpRequested = false;
    private float wallJumpCooldown;
    private int wallSide;

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
        GatherInput();
        CheckEnvironment();
        HandleTimers();
        HandleVisuals();
    }

    private void FixedUpdate()
    {
        HandleMovement();

        if (jumpRequested)
        {
            ExecuteJump();
        }
        else if (wallJumpRequested)
        {
            ExecuteWallJump();
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
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        anim.SetTrigger(JumpHash);
        jumpRequested = false;
    }

    private void ExecuteWallJump()
    {
        float jumpDirection = -wallSide; 
        wallJumpCooldown = wallJumpDuration;
        
        body.linearVelocity = new Vector2(jumpDirection * speed * 1.2f, jumpForce * 0.9f);
        
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
            if (IsGrounded)
            {
                jumpRequested = true;
            }
            else if (OnWall && !IsGrounded)
            {
                wallJumpRequested = true;
            }
        }
    }

    private void HandleTimers()
    {
        if (IsGrounded)
            wallJumpCooldown = 0f;
        else if (wallJumpCooldown > 0f)
            wallJumpCooldown -= Time.deltaTime;
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
            body.gravityScale = 2.5f;

            if (Mathf.Abs(moveInput) > 0.01f)
            {
                body.linearVelocity = new Vector2(moveInput * speed, body.linearVelocity.y);
            }
            return;
        }

        body.gravityScale = 2.5f;
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

    // private void OnDrawGizmos()
    // {
    //     BoxCollider2D col = boxCollider;
    //     if (col == null) col = GetComponent<BoxCollider2D>();
    //     if (col == null) return;

    //     Vector2 bottomPos = new (col.bounds.center.x, col.bounds.min.y);
    //     Vector2 rightPos = new (col.bounds.max.x, col.bounds.center.y);
    //     Vector2 leftPos = new (col.bounds.min.x, col.bounds.center.y);

    //     Vector2 groundSensorSize = new (col.bounds.size.x - 0.05f, 0.1f);
    //     Vector2 wallSensorSize = new (0.1f, col.bounds.size.y * 0.8f);

    //     Gizmos.color = IsGrounded ? Color.green : Color.red;
    //     Gizmos.DrawWireCube(bottomPos, groundSensorSize);

    //     Gizmos.color = (Physics2D.OverlapBox(rightPos, wallSensorSize, 0, wallLayer) != null) ? Color.green : Color.yellow;
    //     Gizmos.DrawWireCube(rightPos, wallSensorSize);

    //     Gizmos.color = (Physics2D.OverlapBox(leftPos, wallSensorSize, 0, wallLayer) != null) ? Color.green : Color.yellow;
    //     Gizmos.DrawWireCube(leftPos, wallSensorSize);
    // }
}
