using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy_Sideways : MonoBehaviour
{
    [SerializeField] private float movementDistance = 3.5f;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float damage = 1f;

    private Rigidbody2D body;
    private bool movingLeft = true;
    private float leftEdge;
    private float rightEdge;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        // body.bodyType = RigidbodyType2D.Kinematic;
        
        leftEdge = transform.position.x - movementDistance;
        rightEdge = transform.position.x + movementDistance;
    }

    private void FixedUpdate()
    {
        float delta = speed * Time.fixedDeltaTime;
        Vector2 pos = body.position;

        if (movingLeft)
        {
            if (pos.x > leftEdge)
                pos.x -= delta;
            else
                movingLeft = false;
        }
        else
        {
            if (pos.x < rightEdge)
                pos.x += delta;
            else
                movingLeft = true;
        }

        body.MovePosition(pos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<Health>(out var playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
