using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DroneAIController : MonoBehaviour
{
    public DroneState currentState = DroneState.Patrol;

    public Transform[] patrolWaypoints;
    public float waypointTolerance = 1.5f;
    private int currentWaypointIndex = 0;

    public Transform playerTransform;
    public float viewDistance = 15f;
    public float viewAngle = 60f;
    public LayerMask obstacleMask;

    public float scanDuration = 3f;
    public float alertInvestigateDuration = 5f;
    private float stateTimer = 0f;
    private Vector3 alertDestination;

    public float empDisableDuration = 30f;

    public float pathUpdateInterval = 0.25f;
    private float nextPathUpdateTime;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.stoppingDistance = 2.0f;
            agent.radius = 0.6f;
            agent.updateRotation = true;
            agent.angularSpeed = 360f;
        }
    }

    private void Start()
    {
        EnsureAgentOnNavMesh();
        if (patrolWaypoints != null && patrolWaypoints.Length > 0)
        {
            SetDestinationOptimized(patrolWaypoints[currentWaypointIndex].position);
        }
    }

    private void Update()
    {
        if (currentState == DroneState.Disabled) return;

        if (currentState == DroneState.Patrol && CanSeePlayerCone())
        {
            TransitionToState(DroneState.Scan);
        }

        switch (currentState)
        {
            case DroneState.Patrol:
                UpdatePatrol();
                break;
            case DroneState.Scan:
                UpdateScan();
                break;
            case DroneState.Alert:
                UpdateAlert();
                break;
            case DroneState.Pursue:
                UpdatePursue();
                break;
        }

        SmoothRotateToTarget();
    }

    private void EnsureAgentOnNavMesh()
    {
        if (agent != null && !agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    private void UpdatePatrol()
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance || agent.remainingDistance <= waypointTolerance)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
                SetDestinationOptimized(patrolWaypoints[currentWaypointIndex].position);
            }
        }
    }

    private void UpdateScan()
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
        
        stateTimer += Time.deltaTime;

        if (CanSeePlayerCone())
        {
            if (agent.isActiveAndEnabled && agent.isOnNavMesh) agent.isStopped = false;
            TransitionToState(DroneState.Pursue);
            return;
        }

        if (stateTimer >= scanDuration)
        {
            if (agent.isActiveAndEnabled && agent.isOnNavMesh) agent.isStopped = false;
            TransitionToState(DroneState.Patrol);
        }
    }

    private void UpdateAlert()
    {
        stateTimer += Time.deltaTime;

        if (CanSeePlayerCone())
        {
            TransitionToState(DroneState.Pursue);
            return;
        }

        if (agent.isActiveAndEnabled && agent.isOnNavMesh && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance || stateTimer >= alertInvestigateDuration)
            {
                TransitionToState(DroneState.Patrol);
            }
        }
    }

    private void UpdatePursue()
    {
        if (CanTrackPlayerInPursuit())
        {
            if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
            SetDestinationOptimized(playerTransform.position);
        }
        else
        {
            alertDestination = playerTransform.position;
            TransitionToState(DroneState.Alert);
            SetDestinationOptimized(alertDestination);
        }
    }

    private void SmoothRotateToTarget()
    {
        if (currentState == DroneState.Pursue && playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    public bool CanSeePlayerCone()
    {
        if (playerTransform == null) return false;

        Vector3 eyeOrigin = transform.position;
        Vector3 targetPoint = playerTransform.position + Vector3.up * 1.0f;

        Vector3 dirToPlayer = (targetPoint - eyeOrigin);
        float distanceToPlayer = dirToPlayer.magnitude;

        if (distanceToPlayer <= viewDistance)
        {
            if (Vector3.Angle(transform.forward, dirToPlayer.normalized) < viewAngle / 2f)
            {
                if (!Physics.Raycast(eyeOrigin, dirToPlayer.normalized, distanceToPlayer, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool CanTrackPlayerInPursuit()
    {
        if (playerTransform == null) return false;

        Vector3 eyeOrigin = transform.position;
        Vector3 targetPoint = playerTransform.position + Vector3.up * 1.0f;

        Vector3 dirToPlayer = (targetPoint - eyeOrigin);
        float distanceToPlayer = dirToPlayer.magnitude;

        if (distanceToPlayer <= viewDistance)
        {
            if (!Physics.Raycast(eyeOrigin, dirToPlayer.normalized, distanceToPlayer, obstacleMask))
            {
                return true;
            }
        }
        return false;
    }

    public void OnNoiseDetected(Vector3 noisePosition)
    {
        if (currentState == DroneState.Disabled || currentState == DroneState.Pursue) return;

        if (currentState == DroneState.Alert && (Time.time < nextPathUpdateTime)) return;

        if (NavMesh.SamplePosition(noisePosition, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            alertDestination = hit.position;
        }
        else
        {
            alertDestination = noisePosition;
        }

        TransitionToState(DroneState.Alert);
        SetDestinationOptimized(alertDestination);
    }

    public void ApplyEMPHit()
    {
        StopAllCoroutines();
        StartCoroutine(DisableSequence());
    }

    private IEnumerator DisableSequence()
    {
        currentState = DroneState.Disabled;
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        yield return new WaitForSeconds(empDisableDuration);

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
        TransitionToState(DroneState.Patrol);
    }

    private void TransitionToState(DroneState newState)
    {
        currentState = newState;
        stateTimer = 0f;

        if (newState == DroneState.Patrol && patrolWaypoints != null && patrolWaypoints.Length > 0)
        {
            if (agent.isActiveAndEnabled && agent.isOnNavMesh) agent.isStopped = false;
            SetDestinationOptimized(patrolWaypoints[currentWaypointIndex].position);
        }
    }

    private void SetDestinationOptimized(Vector3 destination)
    {
        EnsureAgentOnNavMesh();

        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            if (Time.time >= nextPathUpdateTime)
            {
                nextPathUpdateTime = Time.time + pathUpdateInterval;
                if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 4.0f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
                else
                {
                    agent.SetDestination(destination);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 fovLine1 = Quaternion.AngleAxis(viewAngle / 2, transform.up) * transform.forward * viewDistance;
        Vector3 fovLine2 = Quaternion.AngleAxis(-viewAngle / 2, transform.up) * transform.forward * viewDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (playerTransform != null)
        {
            Gizmos.color = CanTrackPlayerInPursuit() ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, playerTransform.position + Vector3.up * 1.0f);
        }
    }
}