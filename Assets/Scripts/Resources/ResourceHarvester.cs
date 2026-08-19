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
    public PlayerZoomCamera playerZoomCamera;
    public UIStateManager uiStateManager;

    [Header("Harvest Detection")]
    [Tooltip("Wie weit der Spieler tatsächlich an eine Ressource heranreichen kann.")]
    public float harvestDistance = 4f;

    [Tooltip("Wie weit die Kamera nach einem möglichen Ziel sucht.")]
    public float cameraSearchDistance = 10f;

    [Tooltip("Player-Layer hier ausschließen.")]
    public LayerMask harvestMask = ~0;

    [Header("Hit Timing")]
    public float axeHitDelayFP = 0.15f;
    public float axeHitDelayTP = 0.33f;

    public float pickaxeHitDelayFP = 0.15f;
    public float pickaxeHitDelayTP = 0.33f;

    [Header("Perspective Resource Balance")]
    [Tooltip("Ressourcenertrag in First Person.")]
    public float firstPersonResourceMultiplier = 1f;

    [Tooltip("Ressourcenertrag in Third Person.")]
    public float thirdPersonResourceMultiplier = 1.25f;

    private Coroutine harvestHitRoutine;

    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (uiStateManager != null &&
            uiStateManager.IsAnyMenuOpen())
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

    void TryHarvest()
    {
        if (playerTool == null ||
            playerCamera == null)
        {
            return;
        }

        bool firstPerson = IsFirstPerson();

        if (playerTool.hasAxe)
        {
            playerTool.PlayAxeSwing();

            if (harvestHitRoutine != null)
                StopCoroutine(harvestHitRoutine);

            float delay =
                firstPerson
                    ? axeHitDelayFP
                    : axeHitDelayTP;

            harvestHitRoutine =
                StartCoroutine(
                    DelayedHarvestHit(delay)
                );
        }
        else if (playerTool.hasPickaxe)
        {
            playerTool.PlayPickaxeSwing();

            if (harvestHitRoutine != null)
                StopCoroutine(harvestHitRoutine);

            float delay =
                firstPerson
                    ? pickaxeHitDelayFP
                    : pickaxeHitDelayTP;

            harvestHitRoutine =
                StartCoroutine(
                    DelayedHarvestHit(delay)
                );
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

        bool firstPerson = IsFirstPerson();

        float resourceMultiplier =
            firstPerson
                ? firstPersonResourceMultiplier
                : thirdPersonResourceMultiplier;

        node.Harvest(
            inventory,
            damage,
            hit.point,
            hit.normal,
            resourceMultiplier
        );
    }

    private bool IsFirstPerson()
    {
        if (playerZoomCamera == null)
            return true;

        return playerZoomCamera.IsFirstPerson;
    }
}