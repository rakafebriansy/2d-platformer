using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthAmount = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<Health>(out var playerHealth))
        {
            if (playerHealth.CurrentHealth < playerHealth.StartingHealth)
            {
                playerHealth.AddHealth(healthAmount);
                Destroy(gameObject);
            }
        }
    }
}
