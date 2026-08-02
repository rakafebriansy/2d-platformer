using System;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator anim;

    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;
    
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private int direction = -1;
    [SerializeField] private float idleDuration = 1f;
    
    private Vector3 initScale;
    private float idleTimer;

    private static readonly int MoveHash = Animator.StringToHash("isMoving");

    private void Awake()
    {
        initScale = enemy.localScale;
        anim = enemy.GetComponent<Animator>();
    }

    private void OnDisable()
    {
        anim.SetBool(MoveHash, false);
    }

    private void Update()
    {
        if (idleTimer > 0)
        {
            idleTimer -= Time.deltaTime;
            anim.SetBool(MoveHash, false);
            return;
        }

        if (direction == -1 && enemy.position.x <= leftEdge.position.x)
        {
            direction = 1;
            idleTimer = idleDuration;
        }
        else if (direction == 1 && enemy.position.x >= rightEdge.position.x)
        {
            direction = -1;
            idleTimer = idleDuration;
        }

        MoveInDirection(direction);
    }

    private void MoveInDirection(int dir)
    {
        anim.SetBool(MoveHash, true);        
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);
        
        enemy.position += Vector3.right * (dir * speed * Time.deltaTime);
    }
}
