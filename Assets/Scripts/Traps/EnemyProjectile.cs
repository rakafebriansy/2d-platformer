using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    private Animator anim;
    private BoxCollider2D boxCollider2D;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float resetTime = 5f;

    private float lifetime;
    private float direction = 1f;
    private bool hit;

    private static readonly int ExplodeHash = Animator.StringToHash("explode");

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public void ActivateProjectile()
    {
        hit = false;
        lifetime = 0;
        gameObject.SetActive(true);
        boxCollider2D.enabled = true;
    }

    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);

        hit = false;
        boxCollider2D.enabled = true;

        float localScaleX = transform.localScale.x;
        if(Mathf.Sign(localScaleX) != direction) 
            localScaleX = -localScaleX;
        
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        base.OnTriggerEnter2D(collision);
        boxCollider2D.enabled = false;

        if (anim != null) 
            anim.SetTrigger(ExplodeHash); //fireballs
        else
            gameObject.SetActive(false); //arrow
    }

    private void Update()
    {
        if (hit) return;

        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed,0,0);

        lifetime += Time.deltaTime;
        if (lifetime > resetTime) 
            Deactivate();
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
