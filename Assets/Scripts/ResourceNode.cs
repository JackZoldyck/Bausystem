using System.Collections;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public enum ResourceType
    {
        Wood,
        Stone
    }
    public ResourceType resourceType;
    public int resourceAmount = 10;
    public Sprite resourceIcon;
    public int health = 3;
    public float respawnTime = 60f;
    public GameObject stumpObject;
    public InventoryGridUI inventoryGridUI;

    private int maxHealth;
    private Renderer[] renderers;
    private Collider[] colliders;
    private TreeHitFeedback feedback;
    private MeshRenderer meshRenderer;
    private Collider treeCollider;
    private bool isDepleted = false;

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
        if (isDepleted)
            return;

        health -= damage;
        if (feedback != null)
            feedback.PlayHitFeedback(hitPoint, hitNormal);

        if (health <= 0)
        {
            isDepleted = true;

            if (resourceType == ResourceType.Wood)
            {
                inventory.wood += resourceAmount;
            }

            if (resourceType == ResourceType.Stone)
            {
                inventory.stone += resourceAmount;
            }

            if (inventoryGridUI != null && resourceIcon != null)
            {
                inventoryGridUI.AddItem(resourceIcon, resourceAmount);
            }

            string resourceName = resourceType == ResourceType.Wood ? "Holz" : "Stein"; 
            
            ResourceGainPopup popup =
                FindFirstObjectByType<ResourceGainPopup>();

            if (popup != null)
            {
                popup.ShowResourceGain(resourceType.ToString(), resourceAmount);
            }

            StartCoroutine(RespawnRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        SetResourceActive(false);

        if (stumpObject != null)
            stumpObject.SetActive(true);

        yield return new WaitForSeconds(respawnTime);

        health = maxHealth;
        isDepleted = false;

        SetResourceActive(true);

        if (stumpObject != null)
            stumpObject.SetActive(false);
    }

    void SetResourceActive(bool active)
    {
        Debug.Log("SetResourceActive: " + active);
        Debug.Log("Renderer Anzahl: " + renderers.Length);
        Debug.Log("Collider Anzahl: " + colliders.Length);

        foreach (Renderer renderer in renderers)
        {
            Debug.Log("Renderer gefunden: " + renderer.name);

            if (stumpObject != null && renderer.transform.IsChildOf(stumpObject.transform))
                continue;

            renderer.enabled = active;
        }

        foreach (Collider collider in colliders)
        {
            Debug.Log("Collider gefunden: " + collider.name);

            if (stumpObject != null && collider.transform.IsChildOf(stumpObject.transform))
                continue;

            collider.enabled = active;
        }
    }
}