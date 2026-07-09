using UnityEngine;

public class SpikeHead : EnemyDamage
{

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float range = 16f;
    [SerializeField] private float checkDelay = 1f;
    
    private Vector3 moveDirection;
    private readonly Vector3[] directions = new Vector3[4];
    private float checkTimer;
    private bool isAttacking;

    private void OnEnable()
    {
        Stop();
    }
    
    private void Update()
    {
        if(isAttacking)
            transform.Translate(moveDirection * (speed * Time.deltaTime), Space.World);
        else
        {
            checkTimer += Time.deltaTime;
            if(checkTimer > checkDelay)
                CheckForPlayer();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        Stop();
    }

    private void CheckForPlayer()
    {
       CalculateDirections();
       for (int i = 0; i < directions.Length; i++)
       {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);

            if(hit.collider != null && !isAttacking)
            {
                isAttacking = true;
                moveDirection = directions[i];
                checkTimer = 0;

                break;
            }
       } 
    }

    private void CalculateDirections()
    {
        directions[0] = transform.right;
        directions[1] = -transform.right;
        directions[2] = transform.up;
        directions[3] = -transform.up;
    }

    private void Stop()
    {
        moveDirection = Vector3.zero;
        isAttacking = false;
    }
}
