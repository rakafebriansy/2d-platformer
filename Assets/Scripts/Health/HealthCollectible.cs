using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthAmount = 1f;
    [SerializeField] private AudioClip healthCollectSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<Health>(out var playerHealth))
        {
            if (playerHealth.CurrentHealth < playerHealth.StartingHealth)
            {
                SoundManager.instance.PlaySound(healthCollectSound);
                playerHealth.AddHealth(healthAmount);
                Destroy(gameObject);
            }
        }
    }
}
