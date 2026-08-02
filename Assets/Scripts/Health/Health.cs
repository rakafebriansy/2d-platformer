using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Health : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Health Settings")]
    [SerializeField] private float startingHealth = 3f;

    [Header("iFrames")]
    [SerializeField] private float invulnerabilityDuration = 2f;
    [SerializeField] private int numberOfFlashes = 5;

    [Header("Components")]
    [SerializeField] private Behaviour[] components;

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
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
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
        {
            anim.SetTrigger(HurtHash);
            StartCoroutine(Invulnerability());
        }
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

        foreach (Behaviour component in components)
            component.enabled = false;

        if (rb != null)
            rb.linearVelocityX = 0f;
    }

    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(10, 11, true);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            sr.color = new Color(1f, 0f, 0f, 0.7f);
            yield return new WaitForSeconds(invulnerabilityDuration / (numberOfFlashes * 2));
            sr.color = Color.white;
            yield return new WaitForSeconds(invulnerabilityDuration / (numberOfFlashes * 2));
        }

        Physics2D.IgnoreLayerCollision(10, 11, false);
    }

    private void Deactivate() 
    {
        gameObject.SetActive(false);
    }
}
