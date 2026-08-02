using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RangedEnemy : MonoBehaviour
{
    private Animator anim;
    private Health playerHealth;
    private EnemyPatrol enemyPatrol;

    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float damageAmount = 2f;
    [SerializeField] private float range = 13f;

    [Header("Ranged Atttack")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;

    [Header("Collider Parameters")]
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private float colliderDistance = 0.45f;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;

    private float cooldownTimer = Mathf.Infinity;

    private static readonly int RangedAttackHash = Animator.StringToHash("rangedAttack");

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    private void Update()
    {
        if (boxCollider2D == null) return;

        cooldownTimer += Time.deltaTime;


        if (PlayerInSight() && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0;
            anim.SetTrigger(RangedAttackHash);
        }

        if(enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }

    private void Attack()
    {
        int bulletIndex = FindFireball();
        if (bulletIndex == -1) return;

        GameObject fireball = fireballs[bulletIndex];
        fireball.transform.position = firePoint.position;
        fireball.GetComponent<EnemyProjectile>().SetDirection(Mathf.Sign(transform.localScale.x));
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

    private bool PlayerInSight()
    {
        if (boxCollider2D == null) return false;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            boxCollider2D.bounds.center + colliderDistance * range * transform.localScale.x * transform.right, 
            new Vector3(boxCollider2D.bounds.size.x * range, boxCollider2D.bounds.size.y, boxCollider2D.bounds.size.z), 
            0, playerLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (boxCollider2D == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider2D.bounds.center + colliderDistance * range * transform.localScale.x * transform.right, 
            new Vector3(boxCollider2D.bounds.size.x * range, boxCollider2D.bounds.size.y, boxCollider2D.bounds.size.z));
    }
}
