using UnityEngine;
using UnityEngine.AI;

public class SkeletonAI : MonoBehaviour
{
    private enum State
    {
        Wander,
        Chase,
        Attack
    }

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Detection")]
    public float detectionRange = 12f;
    public float loseAggroRange = 18f;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    [Header("Wander")]
    public float wanderRadius = 8f;
    public float minIdleTimeBetweenWanders = 5f;
    public float maxIdleTimeBetweenWanders = 10f;

    private bool isWaitingAtWanderPoint;
    private float wanderWaitUntil;

    private State currentState = State.Wander;

    private Vector3 homePosition;

    private float nextAttackTime;

    private bool attackLocked;
    private bool playerDeathHandled;

    private PlayerHealth playerHealth;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (player != null)
        {
            playerHealth =
                player.GetComponentInParent<PlayerHealth>();

            if (playerHealth == null)
                playerHealth =
                    player.GetComponentInChildren<PlayerHealth>();
        }

        homePosition = transform.position;

        if (agent == null)
        {
            Debug.LogError(
                "SkeletonAI: Kein NavMeshAgent gefunden.",
                this
            );

            return;
        }

        if (NavMesh.SamplePosition(
            transform.position,
            out NavMeshHit navHit,
            3f,
            NavMesh.AllAreas))
        {
            agent.Warp(navHit.position);
            homePosition = navHit.position;
        }
        else
        {
            Debug.LogError(
                "SkeletonAI: Kein NavMesh innerhalb von 3 Metern gefunden.",
                this
            );

            return;
        }

        SetNewWanderDestination();
    }

    private void Update()
    {
        if (player == null ||
            agent == null)
        {
            return;
        }

        if (playerHealth != null &&
            playerHealth.IsDead)
        {
            HandlePlayerDeath();

            UpdateWanderWithoutPlayer();
            UpdateAnimator();

            return;
        }

        playerDeathHandled = false;

        float distanceToPlayer =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (!agent.isOnNavMesh)
        {
            if (distanceToPlayer <= attackRange)
            {
                currentState = State.Attack;
                UpdateAttack(distanceToPlayer);
            }

            return;
        }

        switch (currentState)
        {
            case State.Wander:
                UpdateWander(distanceToPlayer);
                break;

            case State.Chase:
                UpdateChase(distanceToPlayer);
                break;

            case State.Attack:
                UpdateAttack(distanceToPlayer);
                break;
        }

        UpdateAnimator();
    }

    private void HandlePlayerDeath()
    {
        if (playerDeathHandled)
            return;

        playerDeathHandled = true;

        attackLocked = false;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
        }

        if (agent != null &&
            agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }

        currentState = State.Wander;

        isWaitingAtWanderPoint = true;

        wanderWaitUntil =
            Time.time +
            Random.Range(
                minIdleTimeBetweenWanders,
                maxIdleTimeBetweenWanders
            );

        Debug.Log(
            "UDO: Spieler ist tot. Kampf beendet."
        );
    }

    private void UpdateWanderWithoutPlayer()
    {
        if (agent == null ||
            !agent.isOnNavMesh)
        {
            return;
        }

        if (isWaitingAtWanderPoint)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            if (Time.time >= wanderWaitUntil)
            {
                isWaitingAtWanderPoint = false;

                SetNewWanderDestination();
            }

            return;
        }

        if (!agent.pathPending &&
            agent.hasPath &&
            agent.remainingDistance <=
            agent.stoppingDistance + 0.1f)
        {
            agent.ResetPath();

            agent.isStopped = true;

            isWaitingAtWanderPoint = true;

            wanderWaitUntil =
                Time.time +
                Random.Range(
                    minIdleTimeBetweenWanders,
                    maxIdleTimeBetweenWanders
                );
        }
    }

    private void UpdateWander(float distanceToPlayer)
    {
        if (distanceToPlayer <= detectionRange)
        {
            isWaitingAtWanderPoint = false;

            currentState =
                State.Chase;

            return;
        }

        if (!agent.isOnNavMesh)
            return;

        if (isWaitingAtWanderPoint)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            if (Time.time >= wanderWaitUntil)
            {
                isWaitingAtWanderPoint = false;

                SetNewWanderDestination();
            }

            return;
        }

        if (!agent.pathPending &&
            agent.hasPath &&
            agent.remainingDistance <=
            agent.stoppingDistance + 0.1f)
        {
            agent.ResetPath();

            agent.isStopped = true;

            isWaitingAtWanderPoint = true;

            wanderWaitUntil =
                Time.time +
                Random.Range(
                    minIdleTimeBetweenWanders,
                    maxIdleTimeBetweenWanders
                );
        }
    }

    private void UpdateChase(float distanceToPlayer)
    {
        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = false;

        if (distanceToPlayer > loseAggroRange)
        {
            currentState = State.Wander;

            SetNewWanderDestination();

            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attack;

            agent.ResetPath();
            agent.isStopped = true;

            return;
        }

        agent.stoppingDistance =
            attackRange * 0.8f;

        agent.SetDestination(
            player.position
        );
    }

    private void UpdateAttack(float distanceToPlayer)
    {
        if (playerHealth != null &&
            playerHealth.IsDead)
        {
            HandlePlayerDeath();
            return;
        }

        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        FacePlayer();

        if (attackLocked)
            return;

        if (distanceToPlayer > attackRange)
        {
            agent.isStopped = false;
            currentState = State.Chase;

            return;
        }

        if (Time.time >= nextAttackTime)
        {
            Debug.Log(
                $"UDO ATTACK | locked={attackLocked} | distance={distanceToPlayer}"
            );

            attackLocked = true;

            if (animator != null)
                animator.SetTrigger("Attack");

            nextAttackTime =
                Time.time + attackCooldown;
        }
    }

    public void FinishAttack()
    {
        Debug.Log(
            "UDO: FinishAttack Event wurde ausgelöst!"
        );

        attackLocked = false;

        if (playerHealth != null &&
            playerHealth.IsDead)
        {
            HandlePlayerDeath();
            return;
        }

        if (agent != null &&
            agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    public void InterruptAttack()
    {
        attackLocked = false;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
        }

        if (agent != null &&
            agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    private void FacePlayer()
    {
        if (player == null)
            return;

        Vector3 direction =
            player.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
    }

    private void SetNewWanderDestination()
    {
        if (agent == null ||
            !agent.isOnNavMesh)
        {
            return;
        }

        agent.isStopped = false;

        Vector3 randomDirection =
            Random.insideUnitSphere *
            wanderRadius;

        randomDirection.y = 0f;

        Vector3 targetPosition =
            homePosition +
            randomDirection;

        if (NavMesh.SamplePosition(
            targetPosition,
            out NavMeshHit hit,
            wanderRadius,
            NavMesh.AllAreas))
        {
            agent.stoppingDistance = 0.2f;

            agent.SetDestination(
                hit.position
            );
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null)
            return;

        float speed = 0f;

        if (agent != null)
            speed = agent.velocity.magnitude;

        animator.SetFloat(
            "Speed",
            speed
        );
    }
}