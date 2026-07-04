using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    private void OnEnable()
    {
        if (health != null)
            health.OnHealthChanged += UpdateHealthUI;
    }

    private void Start()
    {
        UpdateHealthUI();
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnHealthChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI()
    {
        if (health != null && health.StartingHealth > 0)
        {
            currentHealthBar.fillAmount = health.CurrentHealth / 10;
        }
    }
}
