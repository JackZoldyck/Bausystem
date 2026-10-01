using System.Collections;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public enum ResourceType
    {
        Wood,
        Stone
    }

    public enum RequiredTool
    {
        Axe,
        Pickaxe
    }

    public ResourceType resourceType;
    public RequiredTool requiredTool;

    public int resourceAmount = 10;
    public ItemData resourceItem;

    public int health = 3;
    public float respawnTime = 60f;

    public GameObject stumpObject;

    [Header("Physical Drops")]
    public GameObject resourceDropPrefab;
    public float dropRadius = 0.8f;
    public float dropHeight = 0.5f;
    public float dropForce = 2f;

    private int maxHealth;
    private Renderer[] renderers;
    private Collider[] colliders;
    private TreeHitFeedback feedback;
    private bool isDepleted = false;

    void Start()
    {
        maxHealth = health;

        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);

        feedback = GetComponent<TreeHitFeedback>();

        if (stumpObject != null)
            stumpObject.SetActive(false);
    }

    public void Harvest(
        PlayerInventory inventory,
        int damage,
        Vector3 hitPoint,
        Vector3 hitNormal,
        float resourceMultiplier = 1f)
    {
        if (isDepleted)
            return;

        health -= damage;

        if (feedback != null)
        {
            feedback.PlayHitFeedback(
                hitPoint,
                hitNormal
            );
        }

        if (health > 0)
            return;

        isDepleted = true;

        int finalResourceAmount =
            Mathf.Max(
                1,
                Mathf.RoundToInt(
                    resourceAmount * resourceMultiplier
                )
            );

        SpawnResourceDrops(
            finalResourceAmount
        );

        StartCoroutine(
            RespawnRoutine()
        );
    }

    private void SpawnResourceDrops(
        int amount
    )
    {
        if (resourceDropPrefab == null)
        {
            Debug.LogError(
                "ResourceNode: Resource Drop Prefab fehlt!",
                this
            );

            return;
        }

        for (int i = 0; i < amount; i++)
        {
            Vector2 randomCircle =
                Random.insideUnitCircle *
                dropRadius;

            Vector3 spawnPosition =
                transform.position +
                new Vector3(
                    randomCircle.x,
                    dropHeight,
                    randomCircle.y
                );

            Quaternion spawnRotation =
                Random.rotation;

            GameObject drop =
                Instantiate(
                    resourceDropPrefab,
                    spawnPosition,
                    spawnRotation
                );

            PickupItem pickup =
                drop.GetComponent<PickupItem>();

            if (pickup != null)
            {
                pickup.item = resourceItem;
                pickup.amount = 1;
            }

            Rigidbody rb =
                drop.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    IEnumerator RespawnRoutine()
    {
        SetResourceActive(false);

        if (stumpObject != null)
            stumpObject.SetActive(true);

        yield return new WaitForSeconds(
            respawnTime
        );

        health = maxHealth;
        isDepleted = false;

        SetResourceActive(true);

        if (stumpObject != null)
            stumpObject.SetActive(false);
    }

    void SetResourceActive(bool active)
    {
        foreach (Renderer renderer in renderers)
        {
            if (stumpObject != null &&
                renderer.transform.IsChildOf(
                    stumpObject.transform))
            {
                continue;
            }

            renderer.enabled = active;
        }

        foreach (Collider collider in colliders)
        {
            if (stumpObject != null &&
                collider.transform.IsChildOf(
                    stumpObject.transform))
            {
                continue;
            }

            collider.enabled = active;
        }
    }
}