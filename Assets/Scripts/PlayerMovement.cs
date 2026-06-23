using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator anim;
    private bool isGrounded;

    private float moveInput = 0f;
    private bool jumpRequested = false;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;

    private void Awake()
    {
        // grab component references (ecs architecture)
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        moveInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveInput = 1f;

            if ((Keyboard.current.spaceKey.isPressed || Keyboard.current.upArrowKey.isPressed) && isGrounded)
            {
                jumpRequested = true;
            }
        }

        HandleVisuals();
    }

    private void FixedUpdate()
    {
        body.linearVelocity = new Vector2(moveInput * speed, body.linearVelocity.y);

        if (jumpRequested)
        {
            ExecuteJump();
        }
    }

    private void ExecuteJump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);

        anim.SetTrigger("jump");
        
        isGrounded = false;
        jumpRequested = false;
    }

    private void HandleVisuals()
    {
        // flip character left-right
        if(moveInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        } else if (moveInput < -0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        // set animator parameter
        anim.SetBool("isRunning", Mathf.Abs(moveInput) > 0.01f);
        anim.SetBool("isGrounded", isGrounded);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground")) isGrounded = true;
    }
}
