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
}
