using UnityEngine;
using UnityEngine.InputSystem;

public class ArrowTrap : MonoBehaviour
{
    [Header("Attack Settings")] 
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private EnemyProjectile[] arrows;

    private float cooldownTimer = Mathf.Infinity;

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= attackCooldown)
            Attack();
    }

    private void Attack()
    {
        int bulletIndex = FindArrow();
        if (bulletIndex == -1) return;

        cooldownTimer = 0;

        EnemyProjectile arrow = arrows[bulletIndex];
        arrow.transform.position = firePoint.position;
        arrow.GetComponent<EnemyProjectile>().ActivateProjectile();
    }

    private int FindArrow()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] == null) 
            {
                Debug.LogWarning($"[ArrowTrap] Peringatan: Ada slot panah yang kosong di indeks {i}!");
                continue; 
            }
            if(!arrows[i].gameObject.activeInHierarchy)
                return i;
        }
        return -1;
    }
}
