using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    private Animator anim;
    private PlayerMovement playerMovement;

    [Header("Attack Settings")] 
    [SerializeField] private float attackCooldown = 0.25f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;

    private float cooldownTimer = Mathf.Infinity;
    private static readonly int AttackHash = Animator.StringToHash("attack");

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (Keyboard.current == null || Mouse.current == null) return;

        bool isAttackPressed = Keyboard.current.kKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame;

        if (isAttackPressed && cooldownTimer > attackCooldown && playerMovement.CanAttack())
            Attack();
    }

    private void Attack()
    {
        int bulletIndex = FindFireball();

        if (bulletIndex == -1) return;

        anim.SetTrigger(AttackHash);
        cooldownTimer = 0;

        GameObject fireball = fireballs[bulletIndex];
        fireball.transform.position = firePoint.position;
        fireball.GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if(!fireballs[i].activeInHierarchy)
                return i;
        }
        return -1;
    }
}
