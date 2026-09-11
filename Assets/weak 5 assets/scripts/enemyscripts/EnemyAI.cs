using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum AIState { Patrol, Chase, Attack, Stunned, Dead }

    [Header("Current State")]
    public AIState currentState = AIState.Patrol;

    [Header("Detection & Ranges")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.8f;
    [SerializeField] private float attackDamage = 15f;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    [Header("Combat References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask playerLayer;

    private NavMeshAgent agent;
    private Animator animator;
    private EnemyHealth health;
    private Transform player;
    private float lastAttackTime = 0f;
    private bool isKnockedDown = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDeath += HandleDeath;
            health.OnHalfHealth += TriggerHalfHealthKnockdown; // Half health event link
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
            health.OnHalfHealth -= TriggerHalfHealthKnockdown;
        }
    }

    private void Update()
    {
        if (currentState == AIState.Dead || isKnockedDown || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case AIState.Patrol:
                UpdatePatrol(distanceToPlayer);
                break;
            case AIState.Chase:
                UpdateChase(distanceToPlayer);
                break;
            case AIState.Attack:
                UpdateAttack(distanceToPlayer);
                break;
        }

        if (animator != null && agent != null && agent.enabled)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    private void UpdatePatrol(float distance)
    {
        if (distance <= detectionRadius)
        {
            currentState = AIState.Chase;
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0) return;

        agent.isStopped = false;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void UpdateChase(float distance)
    {
        if (distance > detectionRadius * 1.5f)
        {
            currentState = AIState.Patrol;
            return;
        }

        if (distance <= attackRange)
        {
            currentState = AIState.Attack;
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void UpdateAttack(float distance)
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }

        if (distance > attackRange)
        {
            currentState = AIState.Chase;
            agent.isStopped = false;
            return;
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");
        }
    }

    // Normal hits par sirf flinch animation chalegi
    public void ReceiveHit(bool isHeavyKick)
    {
        if (currentState == AIState.Dead || isKnockedDown) return;

        animator.SetTrigger("Hit");

        if (currentState == AIState.Patrol)
        {
            currentState = AIState.Chase;
        }
    }

    // 50% health par ye method call hoga
    private void TriggerHalfHealthKnockdown()
    {
        if (currentState == AIState.Dead) return;
        StartCoroutine(KnockdownRoutine());
    }

    private IEnumerator KnockdownRoutine()
    {
        isKnockedDown = true;
        currentState = AIState.Stunned;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        animator.SetTrigger("Knockdown");

        // Enemy zameen par 4 seconds tak stun rahega
        yield return new WaitForSeconds(4.0f);

        isKnockedDown = false;
        if (agent != null && agent.enabled)
        {
            agent.isStopped = false;
        }
        currentState = AIState.Chase;
    }

    public void EnemyHitCheck()
    {
        Transform point = attackPoint != null ? attackPoint : transform;
        Collider[] hitPlayers = Physics.OverlapSphere(point.position, 1.2f, playerLayer);

        foreach (Collider p in hitPlayers)
        {
            IDamageable target = p.GetComponentInParent<IDamageable>();
            if (target != null && !target.IsDead)
            {
                target.TakeDamage(attackDamage);
            }
        }
    }

    private void HandleDeath()
    {
        if (currentState == AIState.Dead) return;
        currentState = AIState.Dead;

        StopAllCoroutines();

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        yield return new WaitForSeconds(1.5f);

        if (agent != null)
        {
            agent.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackPoint.position, 1.2f);
        }
    }
}