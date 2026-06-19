using System.Collections;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public int woodAmount = 10;
    public int health = 3;
    public float respawnTime = 60f;

    private int maxHealth;
    private Renderer[] renderers;
    private Collider[] colliders;
    private TreeHitFeedback feedback;

    void Start()
    {
        maxHealth = health;
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
        feedback = GetComponent<TreeHitFeedback>();
    }

    public void Harvest(PlayerInventory inventory, int damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        health -= damage;
        if (feedback != null)
            feedback.PlayHitFeedback(hitPoint, hitNormal);

        if (health <= 0)
        {
            inventory.wood += woodAmount;

            InventoryUI inventoryUI = FindFirstObjectByType<InventoryUI>();

            if (inventoryUI != null)
                inventoryUI.UpdateUI();

            StartCoroutine(RespawnRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        SetTreeActive(false);

        yield return new WaitForSeconds(respawnTime);

        health = maxHealth;
        SetTreeActive(true);
    }

    void SetTreeActive(bool active)
    {
        foreach (Renderer renderer in renderers)
            renderer.enabled = active;

        foreach (Collider collider in colliders)
            collider.enabled = active;
    }
}