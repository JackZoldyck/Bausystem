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
    public InventoryGridUI inventoryGridUI;

    private int maxHealth;
    private Renderer[] renderers;
    private Collider[] colliders;
    private TreeHitFeedback feedback;
    private bool isDepleted = false;

    void Awake()
    {
        if (inventoryGridUI == null)
            inventoryGridUI = InventoryGridUI.Instance;
    }

    void Start()
    {
        maxHealth = health;

        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);

        feedback = GetComponent<TreeHitFeedback>();

        if (stumpObject != null)
            stumpObject.SetActive(false);

        if (inventoryGridUI == null)
            inventoryGridUI = InventoryGridUI.Instance;
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

        // Schaden verursachen
        health -= damage;

        // Trefferfeedback bei jedem erfolgreichen Treffer
        if (feedback != null)
        {
            feedback.PlayHitFeedback(
                hitPoint,
                hitNormal
            );
        }

        // Ressource lebt noch
        if (health > 0)
            return;

        // Ressource wurde vollständig abgebaut
        isDepleted = true;

        // Tatsächlichen Ertrag anhand der Perspektive berechnen
        int finalResourceAmount =
            Mathf.Max(
                1,
                Mathf.RoundToInt(
                    resourceAmount * resourceMultiplier
                )
            );

        // Altes PlayerInventory aktualisieren
        if (inventory != null)
        {
            if (resourceType == ResourceType.Wood)
            {
                inventory.wood += finalResourceAmount;
            }

            if (resourceType == ResourceType.Stone)
            {
                inventory.stone += finalResourceAmount;
            }
        }

        // Aktuelles InventoryGrid finden
        InventoryGridUI currentInventory =
            InventoryGridUI.Instance;

        if (currentInventory == null)
        {
            currentInventory =
                FindAnyObjectByType<InventoryGridUI>(
                    FindObjectsInactive.Include
                );
        }

        // Ressource ins sichtbare Inventar legen
        if (currentInventory != null &&
            resourceItem != null)
        {
            currentInventory.AddItem(
                resourceItem,
                finalResourceAmount
            );
        }
        else
        {
            Debug.LogError(
                $"ResourceNode: Ressource konnte nicht ins Inventar gelegt werden. " +
                $"Inventory: {currentInventory}, ResourceItem: {resourceItem}",
                this
            );
        }

        // Popup anzeigen
        ResourceGainPopup popup =
            FindFirstObjectByType<ResourceGainPopup>();

        if (popup != null)
        {
            string resourceName =
                resourceType == ResourceType.Wood
                    ? "Holz"
                    : "Stein";

            popup.ShowResourceGain(
                resourceName,
                finalResourceAmount
            );
        }

        // Respawn starten
        StartCoroutine(RespawnRoutine());
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