using UnityEngine;
using UnityEngine.EventSystems;

public class ResourceHarvester : MonoBehaviour
{
    public Camera playerCamera;
    public float harvestDistance = 4f;
    public PlayerInventory inventory;
    public BuildManager buildManager;
    public PlayerTool playerTool;
    public AxeAnimation axeAnimation;
    public InventoryUI inventoryUI;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (inventoryUI != null && inventoryUI.IsOpen())
            {
                return;
            }

            if (buildManager != null &&
                buildManager.IsBuildModeActive())
            {
                return;
            }

            if (playerTool == null || (!playerTool.hasAxe && !playerTool.hasPickaxe))
                return;

            TryHarvest();
        }
    }

    void TryHarvest()
    {
        if (playerTool == null)
        {
            return;
        }

        if (playerTool.hasAxe)
        {
            axeAnimation?.Swing();
        }
        else if (playerTool.hasPickaxe)
        {
            playerTool.pickaxeAnimation?.Swing();
        }

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, harvestDistance))
        {
            ResourceNode node =
                hit.collider.GetComponentInParent<ResourceNode>();

            if (node != null)
            {
                if (node.requiredTool == ResourceNode.RequiredTool.Axe && !playerTool.hasAxe)
                    return;

                if (node.requiredTool == ResourceNode.RequiredTool.Pickaxe && !playerTool.hasPickaxe)
                    return;

                int damage = playerTool.hasAxe ? playerTool.axeDamage : playerTool.pickaxeDamage;

                node.Harvest(
                    inventory,
                    damage,
                    hit.point,
                    hit.normal
                );
            }
        }

    }
}