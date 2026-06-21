using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField] public float speed = 5f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float moveInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.isPressed || Keyboard.current.upArrowKey.isPressed) {
                body.linearVelocity = new Vector2(body.linearVelocity.x, speed);
            } else {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput = -1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveInput = 1f;
                body.linearVelocity = new Vector2(moveInput * speed, body.linearVelocity.y);
            }
        }
    }
}
