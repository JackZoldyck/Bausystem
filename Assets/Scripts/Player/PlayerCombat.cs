using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public PlayerTool playerTool;
    public PlayerHealth playerHealth;
    public BuildManager buildManager;
    public InventoryUI inventoryUI;
    public UIStateManager uiStateManager;
    public Transform attackPoint;

    [Header("Attack")]
    public float attackCooldown = 0.6f;
    public float hitDelay = 0.18f;

    [Header("Sword Damage")]
    public float swordDamage = 10f;
    public float attackRadius = 1.2f;
    public LayerMask damageMask;

    private float nextAttackTime;

    private void Start()
    {
        if (playerHealth == null)
        {
            playerHealth =
                GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (playerHealth != null &&
            (playerHealth.IsDead ||
             playerHealth.IsRespawning))
        {
            return;
        }

        if (!Input.GetMouseButtonDown(0))
            return;

        if (Time.time < nextAttackTime)
            return;

        if (playerTool == null)
            return;

        if (!playerTool.hasSword)
            return;

        if (buildManager != null &&
            buildManager.IsBuildModeActive())
        {
            return;
        }

        if (inventoryUI != null &&
            inventoryUI.IsOpen())
        {
            return;
        }

        if (uiStateManager != null &&
            uiStateManager.IsAnyMenuOpen())
        {
            return;
        }

        Attack();
    }

    private void Attack()
    {
        if (playerHealth != null &&
            playerHealth.IsDead)
        {
            return;
        }

        nextAttackTime =
            Time.time + attackCooldown;

        playerTool.PlaySwordSwing();

        StartCoroutine(
            SwordHitRoutine()
        );
    }

    private IEnumerator SwordHitRoutine()
    {
        yield return new WaitForSeconds(
            hitDelay
        );

        if (playerHealth != null &&
            playerHealth.IsDead)
        {
            yield break;
        }

        PerformSwordHit();
    }

    private void PerformSwordHit()
    {
        if (playerHealth != null &&
            playerHealth.IsDead)
        {
            return;
        }

        if (attackPoint == null)
        {
            Debug.LogError(
                "PlayerCombat: AttackPoint fehlt!"
            );

            return;
        }

        Collider[] hits =
            Physics.OverlapSphere(
                attackPoint.position,
                attackRadius,
                damageMask,
                QueryTriggerInteraction.Collide
            );

        Debug.Log(
            $"Sword Hit Check: {hits.Length} Collider gefunden"
        );

        HashSet<IDamageable> damagedTargets =
            new HashSet<IDamageable>();

        foreach (Collider hit in hits)
        {
            Debug.Log(
                $"Getroffen: {hit.name} | Layer: {LayerMask.LayerToName(hit.gameObject.layer)}"
            );

            IDamageable damageable =
                hit.GetComponentInParent<IDamageable>();

            if (damageable == null)
            {
                Debug.Log(
                    $"Kein IDamageable bei {hit.name} gefunden"
                );

                continue;
            }

            if (damagedTargets.Contains(
                damageable))
            {
                continue;
            }

            damagedTargets.Add(
                damageable
            );

            Debug.Log(
                $"IDamageable gefunden bei {hit.name}"
            );

            DamageInfo damageInfo =
                new DamageInfo(
                    swordDamage,
                    DamageType.Physical,
                    gameObject
                );

            damageable.TakeDamage(
                damageInfo
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius
        );
    }
}