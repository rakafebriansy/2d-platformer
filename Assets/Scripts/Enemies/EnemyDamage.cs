using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<Health>(out var playerHealth))
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }
}
