using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMovement))]
public class Health : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement playerMovement;

    [Header("Health Settings")]
    [SerializeField] private float startingHealth = 3f;

    private bool isDead;

    public event System.Action OnHealthChanged;

    public float CurrentHealth { get; private set; }
    public float StartingHealth
    {
        get { return startingHealth; }
        set { startingHealth = Mathf.Max(0, value); }
    }

    private static readonly int HurtHash = Animator.StringToHash("hurt");
    private static readonly int DieHash = Animator.StringToHash("die");

    private void Awake()
    {
        CurrentHealth = StartingHealth;
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        #if UNITY_EDITOR
            if (Keyboard.current.eKey.wasPressedThisFrame)
                TakeDamage(1);
        #endif
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, StartingHealth);

        if (CurrentHealth > 0)
            anim.SetTrigger(HurtHash);
        else
            Die();

        OnHealthChanged?.Invoke();
    }

    public void AddHealth(float amount)
    {
        if (isDead) return;
        
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, StartingHealth);
        OnHealthChanged?.Invoke();
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger(DieHash);
        playerMovement.enabled = false;
    }
}
