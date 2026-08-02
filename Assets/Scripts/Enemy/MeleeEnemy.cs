using UnityEngine;

// [RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class MeleeEnemy : MonoBehaviour
{
    private Animator anim;
    private Health playerHealth;
    private EnemyPatrol enemyPatrol;

    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float damageAmount = 2f;
    [SerializeField] private float range = 2.5f;

    [Header("Collider Parameters")]
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private float colliderDistance = 0.45f;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    
    private float cooldownTimer = Mathf.Infinity;

    private static readonly int MeleeAttackHash = Animator.StringToHash("meleeAttack");

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
            anim.SetTrigger(MeleeAttackHash);
        }

        if(enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
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
            {
                playerHealth = hit.transform.GetComponent<Health>();
                return true;
            }
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

    private void DamagePlayer()
    {
        if(PlayerInSight())
            playerHealth.TakeDamage(damageAmount);
    }
}
