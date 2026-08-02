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
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider2D.bounds.center + colliderDistance * range * transform.localScale.x * transform.right, 
            new Vector3(boxCollider2D.bounds.size.x * range, boxCollider2D.bounds.size.y, boxCollider2D.bounds.size.z), 
            0, Vector2.left, 0, playerLayer);

        if(hit.collider != null) 
            playerHealth = hit.transform.GetComponent<Health>();

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
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
