using System.Collections.Generic;
using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [Header("Loot")]
    public ItemData boneItem;
    public int boneAmount = 2;

    public ItemData udoSwordItem;

    [Range(0f, 1f)]
    public float swordDropChance = 0.05f;

    [Header("Prefabs")]
    public LootContainer lootContainerPrefab;
    public GameObject bonePileVisualPrefab;
    public GameObject lootBagVisualPrefab;

    [Header("Ground")]
    public LayerMask groundLayer;

    public void DropLoot()
    {
        if (lootContainerPrefab == null)
        {
            Debug.LogError(
                "EnemyLoot: LootContainer Prefab fehlt!"
            );

            return;
        }

        if (boneItem == null)
        {
            Debug.LogError(
                "EnemyLoot: Bone Item fehlt!"
            );

            return;
        }

        bool dropsSword =
            udoSwordItem != null &&
            Random.value < swordDropChance;

        List<LootEntry> loot =
            new List<LootEntry>();

        loot.Add(
            new LootEntry
            {
                item = boneItem,
                amount = boneAmount
            }
        );

        if (dropsSword)
        {
            loot.Add(
                new LootEntry
                {
                    item = udoSwordItem,
                    amount = 1
                }
            );
        }

        GameObject selectedVisual =
            dropsSword
                ? lootBagVisualPrefab
                : bonePileVisualPrefab;

        Vector3 spawnPosition =
            GetGroundPosition(
                transform.position
            );

        LootContainer container =
            Instantiate(
                lootContainerPrefab,
                spawnPosition,
                Quaternion.identity
            );

        container.Setup(
            loot,
            selectedVisual
        );

        Debug.Log(
            $"Udo Loot: {boneAmount} Knochen | " +
            $"Schwert: {dropsSword}"
        );
    }

    private Vector3 GetGroundPosition(
        Vector3 position
    )
    {
        Vector3 rayStart =
            position + Vector3.up * 5f;

        if (Physics.Raycast(
            rayStart,
            Vector3.down,
            out RaycastHit hit,
            20f,
            groundLayer
        ))
        {
            position.y = hit.point.y;
        }

        return position;
    }
}