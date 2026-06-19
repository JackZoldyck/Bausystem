using UnityEngine;

public class ResourceHarvester : MonoBehaviour
{
    public Camera playerCamera;
    public float harvestDistance = 4f;
    public PlayerInventory inventory;
    public BuildManager buildManager;
    public PlayerTool playerTool;
    public AxeAnimation axeAnimation;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (buildManager != null && buildManager.IsBuildModeActive())
                return;

            TryHarvest();
        }
    }

    void TryHarvest()
    {
        if (axeAnimation != null)
        {
            axeAnimation.Swing();
        }

        Debug.Log("Linksklick erkannt");

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, harvestDistance))
        {
            Debug.Log("Getroffen: " + hit.collider.name);

            ResourceNode node = hit.collider.GetComponentInParent<ResourceNode>();

            if (node != null)
            {
                if (playerTool == null || !playerTool.hasAxe)
                    return;

                node.Harvest(inventory, playerTool.axeDamage);
            }
        }
        else
        {
            Debug.Log("Nichts getroffen");
        }
    }
}