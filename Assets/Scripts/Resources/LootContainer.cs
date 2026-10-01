using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootEntry
{
    public ItemData item;
    public int amount;
}

public class LootContainer : MonoBehaviour
{
    [Header("Loot")]
    public List<LootEntry> loot = new List<LootEntry>();

    [Header("World Visual")]
    public GameObject worldVisual;

    public void Setup(
        List<LootEntry> newLoot,
        GameObject visualPrefab
    )
    {
        loot.Clear();

        foreach (LootEntry entry in newLoot)
        {
            if (entry.item == null || entry.amount <= 0)
                continue;

            loot.Add(
                new LootEntry
                {
                    item = entry.item,
                    amount = entry.amount
                }
            );
        }

        if (worldVisual != null)
        {
            Destroy(worldVisual);
        }

        if (visualPrefab != null)
        {
            worldVisual = Instantiate(
                visualPrefab,
                transform
            );

            worldVisual.transform.localPosition =
                Vector3.zero;

            worldVisual.transform.localRotation =
                Quaternion.identity;
        }
    }

    public string GetContentText()
    {
        List<string> contentParts =
            new List<string>();

        foreach (LootEntry entry in loot)
        {
            if (entry.item == null ||
                entry.amount <= 0)
            {
                continue;
            }

            contentParts.Add(
                $"{entry.amount}x {entry.item.itemName}"
            );
        }

        return string.Join(
            ", ",
            contentParts
        );
    }

    public string GetPromptText()
    {
        return "[E] Aufheben";
    }

    public bool Pickup(
        InventoryGridUI inventoryGrid
    )
    {
        if (inventoryGrid == null)
            return false;

        bool everythingCollected = true;

        for (int i = loot.Count - 1; i >= 0; i--)
        {
            LootEntry entry = loot[i];

            if (entry.item == null ||
                entry.amount <= 0)
            {
                loot.RemoveAt(i);
                continue;
            }

            int addedAmount =
                inventoryGrid.AddItem(
                    entry.item,
                    entry.amount
                );

            entry.amount -= addedAmount;

            if (entry.amount <= 0)
            {
                loot.RemoveAt(i);
            }
            else
            {
                everythingCollected = false;
            }
        }

        if (loot.Count == 0)
        {
            Destroy(gameObject);
            return true;
        }

        return everythingCollected;
    }
}