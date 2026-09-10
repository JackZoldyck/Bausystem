using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("References")]
    public PlayerStats playerStats;
    public Animator animator;

    public bool IsDead => isDead;
    public DeathScreenUI deathScreenUI;

    private bool isDead;

    private bool isRespawning;
    public bool IsRespawning => isRespawning;

    private int toolSwingLayerIndex;

    private void Start()
    {
        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();

        if (animator != null)
            toolSwingLayerIndex = animator.GetLayerIndex("ToolSwing");
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (isDead || playerStats == null)
            return;

        playerStats.currentHealth -= damageInfo.amount;
        playerStats.currentHealth = Mathf.Max(playerStats.currentHealth, 0f);

        Debug.Log($"Player erhält {damageInfo.amount} Schaden. HP: {playerStats.currentHealth}/{playerStats.GetMaxHealth()}");

        if (playerStats.currentHealth <= 0f)
        {
            Die();
        }

        else
        {
            if (animator != null)
            {
                Debug.Log("PLAYER HIT TRIGGER!");

                if (toolSwingLayerIndex >= 0)
                    animator.SetLayerWeight(toolSwingLayerIndex, 0f);

                animator.SetTrigger("Hit");
            }
        }
    }

    public void FinishHitReaction()
    {
        if (isDead || animator == null)
            return;

        if (toolSwingLayerIndex >= 0)
            animator.SetLayerWeight(toolSwingLayerIndex, 1f);
    }

    private void PlayHitAnimation()
    {
        if (animator == null)
            return;

        animator.SetTrigger("Hit");
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Player ist gestorben.");

        if (animator != null)
        {
            if (toolSwingLayerIndex >= 0)
                animator.SetLayerWeight(toolSwingLayerIndex, 0f);

            animator.SetTrigger("Death");
        }

        if (deathScreenUI != null)
        {
            deathScreenUI.ShowDeathScreen();
        }
    }

    public void Respawn()
    {
        isDead = false;
        isRespawning = true;

        if (playerStats != null)
        {
            playerStats.currentHealth =
                playerStats.GetMaxHealth();
        }

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);

            animator.ResetTrigger("Death");
            animator.ResetTrigger("Hit");

            if (toolSwingLayerIndex >= 0)
            {
                animator.SetLayerWeight(
                    toolSwingLayerIndex,
                    1f
                );
            }

            animator.SetTrigger("Respawn");
        }

        Debug.Log(
            $"Player respawned. HP: {playerStats.currentHealth}/{playerStats.GetMaxHealth()}"
        );

    Debug.Log(
            $"Player respawned. HP: {playerStats.currentHealth}/{playerStats.GetMaxHealth()}"
    );
    }

    public void FinishRespawn()
    {
        isRespawning = false;

        Debug.Log("PLAYER: Respawn abgeschlossen");
    }

}