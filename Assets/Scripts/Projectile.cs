using UnityEngine;
[RequireComponent(typeof(Animator))]

[RequireComponent(typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour
{
    [Header("References")]
    private Animator anim;
    private BoxCollider2D boxCollider;

    [Header("Projectile Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxLifetime = 2.5f;

    private float direction;
    private bool hit;
    private float lifetime;

    private static readonly int ExplodeHash = Animator.StringToHash("explode");

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (hit) return;

        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed,0,0);

        lifetime += Time.deltaTime;
        if (lifetime > maxLifetime) 
            Deactivate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger(ExplodeHash);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void SetDirection(float direction)
    {
        lifetime = 0;

        this.direction = direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if(Mathf.Sign(localScaleX) != direction) 
            localScaleX = -localScaleX;
        
        transform.localScale = new (localScaleX, transform.localScale.y, transform.localScale.z);
    }
}
