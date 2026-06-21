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
                Debug.Log("Blockiert: Inventar offen");
                return;
            }

            if (buildManager != null &&
                buildManager.IsBuildModeActive())
            {
                Debug.Log("Blockiert: Baumodus aktiv");
                return;
            }

            if (playerTool == null || !playerTool.hasAxe)
                return;

            TryHarvest();
        }
    }

    void TryHarvest()
    {
        if (playerTool == null)
        {
            Debug.Log("PlayerTool Referenz ist NULL");
            return;
        }

        if (!playerTool.hasAxe)
        {
            Debug.Log("Axt nicht ausgerüstet");
            return;
        }

        if (axeAnimation != null)
            axeAnimation.Swing();

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
                node.Harvest(
                    inventory,
                    playerTool.axeDamage,
                    hit.point,
                    hit.normal
                );
            }
        }

    }
}