using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [Header("Loot")]
    public int boneAmount = 2;

    [Range(0f, 1f)]
    public float swordDropChance = 0.05f;

    [Header("Loot Prefabs")]
    public GameObject boneLootPrefab;
    public GameObject swordLootPrefab;

    public void DropLoot()
    {
        if (boneLootPrefab != null &&
            boneAmount > 0)
        {
            for (int i = 0; i < boneAmount; i++)
            {
                Instantiate(
                    boneLootPrefab,
                    transform.position,
                    Quaternion.identity
                );
            }
        }

        if (swordLootPrefab != null &&
            Random.value <= swordDropChance)
        {
            Instantiate(
                swordLootPrefab,
                transform.position,
                Quaternion.identity
            );

            Debug.Log("UDO DROPPED HIS SWORD!");
        }
    }
}