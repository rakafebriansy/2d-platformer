using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Firetrap : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;

    [Header("Firetrap Damage")]
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private float damageTickRate = 0.5f;

    [Header("Firetrap Timers")]
    [SerializeField] private float activationDelay = 2f;
    [SerializeField] private float activeDuration = 2f;

    private Health playerHealth;
    private bool isTriggered;
    private bool isActive;

    private static readonly int ActivatedHash = Animator.StringToHash("activated");

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.TryGetComponent(out playerHealth);

            if (!isTriggered)
                StartCoroutine(ActivateFiretrap());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out playerHealth))
            playerHealth = null;
    }

    private IEnumerator ActivateFiretrap()
    {
        isTriggered = true;
        sr.color = Color.red;

        yield return new WaitForSeconds(activationDelay);
        sr.color = Color.white;
        isActive = true;
        anim.SetBool(ActivatedHash, true);

        float activeTimer = 0f;
        float nextDamageTime = 0f;

        while (activeTimer < activeDuration)
        {
            if (activeTimer >= nextDamageTime && playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                nextDamageTime += damageTickRate;
            }
            activeTimer += Time.deltaTime;
            yield return null;
        }

        isActive = false;
        isTriggered = false;
        anim.SetBool(ActivatedHash, false);

        playerHealth = null;
    }
}
