using UnityEngine;
using System.Collections;

public class ResourceHarvester : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public PlayerInventory inventory;
    public BuildManager buildManager;
    public PlayerTool playerTool;
    public InventoryUI inventoryUI;

    [Header("Harvest Detection")]
    [Tooltip("Wie weit der Spieler tatsächlich an eine Ressource heranreichen kann.")]
    public float harvestDistance = 4f;

    [Tooltip("Wie weit die Kamera nach einem möglichen Ziel sucht.")]
    public float cameraSearchDistance = 10f;

    [Tooltip("Player-Layer hier ausschließen.")]
    public LayerMask harvestMask = ~0;

    [Header("Hit Timing")]
    public float axeHitDelay = 0.33f;
    public float pickaxeHitDelay = 0.33f;

    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (inventoryUI != null &&
            inventoryUI.IsOpen())
        {
            return;
        }

        if (buildManager != null &&
            buildManager.IsBuildModeActive())
        {
            return;
        }

        if (playerTool == null ||
            (!playerTool.hasAxe &&
             !playerTool.hasPickaxe))
        {
            return;
        }

        TryHarvest();
    }

    private Coroutine harvestHitRoutine;

    void TryHarvest()
    {
        if (playerTool == null || playerCamera == null)
            return;

        if (playerTool.hasAxe)
        {
            playerTool.PlayAxeSwing();

            if (harvestHitRoutine != null)
                StopCoroutine(harvestHitRoutine);

            harvestHitRoutine =
                StartCoroutine(DelayedHarvestHit(axeHitDelay));
        }
        else if (playerTool.hasPickaxe)
        {
            playerTool.PlayPickaxeSwing();

            if (harvestHitRoutine != null)
                StopCoroutine(harvestHitRoutine);

            harvestHitRoutine =
                StartCoroutine(DelayedHarvestHit(pickaxeHitDelay));
        }
    }

    IEnumerator DelayedHarvestHit(float delay)
    {
        yield return new WaitForSeconds(delay);

        ApplyHarvestHit();

        harvestHitRoutine = null;
    }

    public void ApplyHarvestHit()
    {
        Debug.Log(
            $"HARVEST HIT | Objekt: {gameObject.name} | ID: {GetInstanceID()} | Frame: {Time.frameCount} | AxeDamage: {playerTool.axeDamage}"
        );

        if (playerTool == null ||
            playerCamera == null)
        {
            return;
        }

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            cameraSearchDistance,
            harvestMask,
            QueryTriggerInteraction.Ignore))
        {
            return;
        }

        ResourceNode node =
            hit.collider.GetComponentInParent<ResourceNode>();

        if (node == null)
            return;

        float distanceFromPlayer =
            Vector3.Distance(
                transform.position,
                hit.point
            );

        if (distanceFromPlayer > harvestDistance)
            return;

        if (node.requiredTool ==
                ResourceNode.RequiredTool.Axe &&
            !playerTool.hasAxe)
        {
            return;
        }

        if (node.requiredTool ==
                ResourceNode.RequiredTool.Pickaxe &&
            !playerTool.hasPickaxe)
        {
            return;
        }

        int damage =
            playerTool.hasAxe
                ? playerTool.axeDamage
                : playerTool.pickaxeDamage;

        node.Harvest(
            inventory,
            damage,
            hit.point,
            hit.normal
        );
    }
}