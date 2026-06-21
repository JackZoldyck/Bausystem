using System.Collections;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public int woodAmount = 10;
    public int health = 3;
    public float respawnTime = 60f;
    public GameObject stumpObject;

    private int maxHealth;
    private Renderer[] renderers;
    private Collider[] colliders;
    private TreeHitFeedback feedback;
    private MeshRenderer meshRenderer;
    private Collider treeCollider;

    void Start()
    {
        maxHealth = health;
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
        feedback = GetComponent<TreeHitFeedback>();
        meshRenderer = GetComponent<MeshRenderer>();
        treeCollider = GetComponent<Collider>();

        if (stumpObject != null)
            stumpObject.SetActive(false);
    }

    public void Harvest(PlayerInventory inventory, int damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        health -= damage;
        if (feedback != null)
            feedback.PlayHitFeedback(hitPoint, hitNormal);

        if (health <= 0)
        {
            inventory.wood += woodAmount;

            WoodGainPopup popup =
                FindFirstObjectByType<WoodGainPopup>();

            if (popup != null)
            {
                popup.ShowWoodGain(woodAmount);
            }

            InventoryUI inventoryUI = FindFirstObjectByType<InventoryUI>();

            if (inventoryUI != null)
                inventoryUI.UpdateUI();

            StartCoroutine(RespawnRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        SetTreeActive(false);

        if (stumpObject != null)
            stumpObject.SetActive(true);

        yield return new WaitForSeconds(respawnTime);

        health = maxHealth;

        SetTreeActive(true);

        if (stumpObject != null)
            stumpObject.SetActive(false);
    }

    void SetTreeActive(bool active)
    {
        foreach (Renderer renderer in renderers)
        {
            if (stumpObject != null && renderer.transform.IsChildOf(stumpObject.transform))
                continue;

            renderer.enabled = active;
        }

        foreach (Collider collider in colliders)
        {
            if (stumpObject != null && collider.transform.IsChildOf(stumpObject.transform))
                continue;

            collider.enabled = active;
        }
    }
}