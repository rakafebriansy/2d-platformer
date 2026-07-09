using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float resetTime = 5f;
    private float lifetime;

    public void ActivateProjectile()
    {
        lifetime = 0;
        gameObject.SetActive(true);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        float movementSpeed = speed * Time.deltaTime;
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
