using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;

    [Header("Damage")]
    public float damage = 10f;
    public float attackRadius = 1.2f;
    public LayerMask damageMask;

    public void ApplyAttackDamage()
    {
        Debug.Log("UDO: ApplyAttackDamage wurde ausgelöst");

        if (attackPoint == null)
        {
            Debug.LogError("UDO: AttackPoint fehlt!");
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
            $"UDO Hit Check: {hits.Length} Collider gefunden"
        );

        foreach (Collider hit in hits)
        {
            Debug.Log(
                $"UDO trifft Collider: {hit.name} | Layer: " +
                LayerMask.LayerToName(hit.gameObject.layer)
            );

            IDamageable damageable =
                hit.GetComponentInParent<IDamageable>();

            if (damageable == null)
            {
                Debug.Log(
                    $"UDO: Kein IDamageable bei {hit.name}"
                );

                continue;
            }

            DamageInfo damageInfo =
                new DamageInfo(
                    damage,
                    DamageType.Physical,
                    gameObject
                );

            damageable.TakeDamage(
                damageInfo
            );

            Debug.Log("UDO: Schaden angewendet");

            break;
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