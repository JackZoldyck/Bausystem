using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 50f;

    [Header("References")]
    public Animator animator;
    public NavMeshAgent agent;
    public SkeletonAI enemyAI;

    private float currentHealth;
    private bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (enemyAI == null)
            enemyAI = GetComponent<SkeletonAI>();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (isDead)
            return;

        currentHealth -= damageInfo.amount;

        currentHealth =
            Mathf.Max(
                currentHealth,
                0f
            );

        Debug.Log(
            $"{gameObject.name} erhält {damageInfo.amount} Schaden. HP: {currentHealth}/{maxHealth}"
        );

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        if (enemyAI != null)
        {
            enemyAI.InterruptAttack();
        }

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (enemyAI != null)
            enemyAI.enabled = false;

        if (agent != null &&
            agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }
}