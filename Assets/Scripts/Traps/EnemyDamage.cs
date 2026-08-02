using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] protected float damageAmount = 1f;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<Health>(out var playerHealth))
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent<Health>(out var playerHealth))
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }
}
